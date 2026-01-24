
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HachigayoLab.PolychoraViewer
{
    public class UniformScreen : IScreen
    {
        Mesh mesh;
        Material material;
        Vector2[][] vertices;
        Vector2[][] centers;
        Vector2[][] uvs;
        int vertexCount;
        Vector4 vertex = Vector4.zero;

        Vector3 reg1 = Vector3.zero;
        Vector3 reg2 = Vector3.zero;

        [SerializeField] bool zRotation = true;
        [SerializeField] bool inertial = true;
        [SerializeField] public Matrix4x4 axis;
        Matrix4x4 _axis;
        [SerializeField] public Matrix4x4 rotate;

        float radius, fov, tanFov, distance;

        bool invertNormals = false;
        float scaleCache = 0.25f;
        bool glass = false;
        int vis = 0b1111;
        float cellSize = 0.5f;
        float edgeSize = 0.05f;
        float initFov = 15f;
        float border = -1;
        float speedW = 1;
        float speedNonW = 1;
        bool wRotation = true;
        bool nonWRotation = true;
        bool inertialActiveW = false, inertialActiveNonW = false;

        Vector3 head;
        [SerializeField] Slider sliderCell, sliderEdge, sliderXray, sliderFov, sliderScale, sliderSpeedW, sliderSpeedNonW;
        [SerializeField] Toggle toggleFlip, toggleGlass, toggleLock, toggleWRotation, toggleNonWRotation, toggleInertial;
        [SerializeField] Toggle[] toggleVis;
        [SerializeField] GameObject controller, menu, domainManagerTransform;
        [SerializeField] DomainManager domainManager;
        MeshRenderer[] domainRenderers;
        Collider domainCollider;
        [SerializeField] InputField inputFieldAxis, inputFieldRotate;
        Syncer syncer;
        int uiState = 0;
        bool beforeUpdate = true;

        public override void OnPickupUseDown() { Interact(); }

        public override void Interact()
        {
            uiState = (uiState + 1) % 4;
            Vector3 lookAt = transform.position - head;
            lookAt.y = 0;
            if (uiState == 0)
            {
                controller.SetActive(false);
                foreach (MeshRenderer mr in domainRenderers) mr.enabled = false;
                domainCollider.enabled = false;
                menu.SetActive(false);
            }
            else if (uiState == 1)
            {
                controller.SetActive(true);
                foreach (MeshRenderer mr in domainRenderers) mr.enabled = false;
                domainCollider.enabled = false;
                menu.SetActive(false);
                // if (zRotation) controller.transform.rotation = Quaternion.LookRotation(lookAt, Vector3.up);
            }
            else if (uiState == 2)
            {
                controller.SetActive(true);
                foreach (MeshRenderer mr in domainRenderers) mr.enabled = true;
                domainCollider.enabled = true;
                menu.SetActive(false);
                // if (zRotation) controller.transform.rotation = domainManagerTransform.transform.rotation = Quaternion.LookRotation(lookAt, Vector3.up);
            }
            else
            {
                controller.SetActive(true);
                // domainManagerTransform.SetActive(true);
                foreach (MeshRenderer mr in domainRenderers) mr.enabled = true;
                domainCollider.enabled = true;
                menu.SetActive(true);
                // if (zRotation) controller.transform.rotation = domainManagerTransform.transform.rotation = menu.transform.rotation = Quaternion.LookRotation(lookAt, Vector3.up);
            }
        }

        public override void UpdateAll()
        {
            UpdateScale();
            UpdateCell();
            UpdateEdge();
            UpdateXray();
            UpdateFov();
            UpdateFlip();
            UpdateGlass();
            UpdateVis();
            UpdateColors();
            SyncAxis();
        }

        float scaleTimer;
        bool scaleIndirect;
        public void UpdateScale()
        {
            if (scaleIndirect) { scaleIndirect = false; return; }
            scaleCache = sliderScale.value;
            scaleTimer = 0.5f;
        }
        public override void ScaleChanged()
        {
            transform.localScale = Vector3.one * 0.025f * Mathf.Pow(10, syncer.Scale);
            if (!beforeUpdate && sliderScale.value != syncer.Scale)
            {
                scaleIndirect = true;
                sliderScale.value = syncer.Scale;
            }
        }

        bool cellIndirect;
        public void UpdateCell()
        {
            if (cellIndirect) { cellIndirect = false; return; }
            syncer.SetCell(sliderCell.value);
        }
        public override void CellChanged()
        {
            cellSize = syncer.Cell;
            SetVertices();
            if (!beforeUpdate && sliderCell.value != syncer.Cell)
            {
                cellIndirect = true;
                sliderCell.value = syncer.Cell;
            }
        }

        bool edgeIndirect;
        public void UpdateEdge()
        {
            if (edgeIndirect) { edgeIndirect = false; return; }
            syncer.SetEdge(sliderEdge.value);
        }
        public override void EdgeChanged()
        {
            edgeSize = syncer.Edge;
            SetVertices();
            if (!beforeUpdate && sliderEdge.value != syncer.Edge)
            {
                edgeIndirect = true;
                sliderEdge.value = syncer.Edge;
            }
        }

        bool xrayIndirect;
        public void UpdateXray()
        {
            if (xrayIndirect) { xrayIndirect = false; return; }
            syncer.SetXray(sliderXray.value);
        }
        public override void XrayChanged()
        {
            border = syncer.Xray;
            if (!beforeUpdate && sliderXray.value != syncer.Xray)
            {
                xrayIndirect = true;
                sliderXray.value = syncer.Xray;
            }
        }

        bool fovIndirect;
        public void UpdateFov()
        {
            if (fovIndirect) { fovIndirect = false; return; }
            syncer.SetFov(sliderFov.value);
        }
        public override void FovChanged()
        {
            SetFov(syncer.Fov * Mathf.PI / 180f);
            if (!beforeUpdate && sliderFov.value != syncer.Fov)
            {
                fovIndirect = true;
                sliderFov.value = syncer.Fov;
            }
        }

        bool flipIndirect;
        public void UpdateFlip()
        {
            if (flipIndirect) { flipIndirect = false; return; }
            syncer.SetFlip(toggleFlip.isOn);
        }
        public override void FlipChanged()
        {
            invertNormals = syncer.Flip;
            if (!beforeUpdate && toggleFlip.isOn != syncer.Flip)
            {
                flipIndirect = true;
                toggleFlip.isOn = syncer.Flip;
            }
        }

        bool glassIndirect;
        public void UpdateGlass()
        {
            if (glassIndirect) { glassIndirect = false; return; }
            syncer.SetGlass(toggleGlass.isOn);
        }
        public override void GlassChanged()
        {
            glass = syncer.Glass;
            if (!beforeUpdate && toggleGlass.isOn != syncer.Glass)
            {
                glassIndirect = true;
                toggleGlass.isOn = syncer.Glass;
            }
        }

        bool visIndirect;
        public void UpdateVis()
        {
            if (visIndirect) { visIndirect = false; return; }
            int v = 0;
            for (int i = 0; i < toggleVis.Length; i++) if (toggleVis[i].isOn) v += 1 << i;
            syncer.SetVis(v);
        }
        public override void VisChanged()
        {
            vis = syncer.Vis;
            int v = 0;
            for (int i = 0; i < toggleVis.Length; i++) if (toggleVis[i].isOn) v += 1 << i;
            if (!beforeUpdate && v != syncer.Vis)
            {
                visIndirect = true;
                for (int i = 0; i < toggleVis.Length; i++) toggleVis[i].isOn = ((syncer.Vis >> i) & 1) > 0;
            }
        }

        public void UpdateColors()
        {
            syncer.SetColors(new Color[] {});
        }
        public override void ColorsChanged() {}

        bool wRotationIndirect;
        public void UpdateWRotation()
        {
            if (wRotationIndirect) { wRotationIndirect = false; return; }
            syncer.SetWRotation(toggleWRotation.isOn);
        }
        public override void WRotationChanged()
        {
            wRotation = syncer.WRotation;
            if (!beforeUpdate && toggleWRotation.isOn != syncer.WRotation)
            {
                wRotationIndirect = true;
                toggleWRotation.isOn = syncer.WRotation;
            }
        }

        bool nonWRotationIndirect;
        public void UpdateNonWRotation()
        {
            if (nonWRotationIndirect) { nonWRotationIndirect = false; return; }
            syncer.SetNonWRotation(toggleNonWRotation.isOn);
        }
        public override void NonWRotationChanged()
        {
            nonWRotation = syncer.NonWRotation;
            if (!beforeUpdate && toggleNonWRotation.isOn != syncer.NonWRotation)
            {
                nonWRotationIndirect = true;
                toggleNonWRotation.isOn = syncer.NonWRotation;
            }
        }

        bool inertialIndirect;
        public void UpdateInertial()
        {
            if (inertialIndirect) { inertialIndirect = false; return; }
            syncer.SetInertial(toggleInertial.isOn);
        }
        public override void InertialChanged()
        {
            inertial = syncer.Inertial;
            inertialActiveW = false;
            inertialActiveNonW = false;
            Vector4 v;
            v = new Vector4(0, 0, 0, 1);
            for (int i = 0; i < 4; i++) rotate[i, 1] = v[i];
            v = new Vector4(1, 0, 0, 0);
            for (int i = 0; i < 4; i++) rotate[i, 2] = v[i];
            v = new Vector4(1, 0, 0, 0);
            for (int i = 0; i < 4; i++) rotate[i, 3] = v[i];
            if (!beforeUpdate && toggleInertial.isOn != syncer.Inertial)
            {
                inertialIndirect = true;
                toggleInertial.isOn = syncer.Inertial;
            }
        }
        public override void OnGrab() { inertialActiveW = inertial && !wRotation; inertialActiveNonW = inertial && !nonWRotation; }
        public override void OnRelease()
        {
            inertialActiveW = inertial;
            inertialActiveNonW = inertial;
            SyncAxis();
            if (inertial) SyncRotate(); 
        }

        bool speedWIndirect;
        public void UpdateSpeedW()
        {
            if (speedWIndirect) { speedWIndirect = false; return; }
            syncer.SetSpeedW(sliderSpeedW.value);
        }
        public override void SpeedWChanged()
        {
            speedW = syncer.SpeedW;
            if (!beforeUpdate && sliderSpeedW.value != syncer.SpeedW)
            {
                speedWIndirect = true;
                sliderSpeedW.value = syncer.SpeedW;
            }
        }

        bool speedNonWIndirect;
        public void UpdateSpeedNonW()
        {
            if (speedNonWIndirect) { speedNonWIndirect = false; return; }
            syncer.SetSpeedNonW(sliderSpeedNonW.value);
        }
        public override void SpeedNonWChanged()
        {
            speedNonW = syncer.SpeedNonW;
            if (!beforeUpdate && sliderSpeedNonW.value != syncer.SpeedNonW)
            {
                speedNonWIndirect = true;
                sliderSpeedNonW.value = syncer.SpeedNonW;
            }
        }

        public void Reset()
        {
            _axis = axis;
            SyncAxis();
        }

        public void ImportAxis()
        {
            string[] s = inputFieldAxis.text.Split(',');
            if (s.Length != 16) return;
            Matrix4x4 m = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (float.TryParse(s[i*4+j], out float value)) m[i, j] = value;
                    else return;
            _axis = m;
            SyncAxis();
        }

        public void ExportAxis()
        {
            string s = "";
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    s += _axis[i, j].ToString("R") + ",";
            inputFieldAxis.text = s.Remove(s.Length - 1);
        }

        public void ImportRotate()
        {
            string[] s = inputFieldRotate.text.Split(',');
            if (s.Length != 16) return;
            Matrix4x4 m = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (float.TryParse(s[i*4+j], out float value)) m[i, j] = value;
                    else return;
            rotate = m;
            SyncRotate();
            toggleInertial.isOn = true;
            UpdateInertial();
        }

        public void ExportRotate()
        {
            string s = "";
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    s += rotate[i, j].ToString("R") + ",";
            inputFieldRotate.text = s.Remove(s.Length - 1);
        }

        bool axisChanged;
        public void SyncAxis()
        {
            syncer.SetAxis(_axis.GetColumn(0), _axis.GetColumn(1), _axis.GetColumn(2), _axis.GetColumn(3));
        }
        public override void AxisChanged()
        {
            axisChanged = true;
        }
        void AxisUpdate()
        {
            Vector4 v = syncer.Axis0;
            for (int i = 0; i < 4; i++) _axis[i, 0] = v[i];
            v = syncer.Axis1;
            for (int i = 0; i < 4; i++) _axis[i, 1] = v[i];
            v = syncer.Axis2;
            for (int i = 0; i < 4; i++) _axis[i, 2] = v[i];
            v = syncer.Axis3;
            for (int i = 0; i < 4; i++) _axis[i, 3] = v[i];
            axisChanged = false;
        }

        bool rotateChanged;
        public void SyncRotate()
        {
            syncer.SetRotate(rotate.GetColumn(0), rotate.GetColumn(1), rotate.GetColumn(2), rotate.GetColumn(3));
        }
        public override void RotateChanged()
        {
            rotateChanged = true;
        }
        void RotateUpdate()
        {
            Vector4 v = syncer.Rotate0;
            for (int i = 0; i < 4; i++) rotate[i, 0] = v[i];
            v = syncer.Rotate1;
            for (int i = 0; i < 4; i++) rotate[i, 1] = v[i];
            v = syncer.Rotate2;
            for (int i = 0; i < 4; i++) rotate[i, 2] = v[i];
            v = syncer.Rotate3;
            for (int i = 0; i < 4; i++) rotate[i, 3] = v[i];
            rotateChanged = false;
            inertialActiveW = true;
            inertialActiveNonW = true;
        }

        public void SetFov(float fov)
        {
            this.fov = Mathf.Min(fov, Mathf.PI / 2 * 0.99999999f);
            if (fov <= 0) return;
            distance = radius / Mathf.Sin(fov);
            tanFov = Mathf.Tan(fov);
        }

        public override void CalcInput(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            Vector4 v;
            if (wRotation)
            {
                if (!zRotation) deltaPosition.z = 0;
                float t = Mathf.Sqrt(Mathf.Sqrt(2 * Mathf.PI * deltaPosition.magnitude)) * 4f * transform.localScale.x * speedW;
                deltaPosition.Normalize();
                v = new Vector4(-deltaPosition[0] * Mathf.Sin(t), -deltaPosition[1] * Mathf.Sin(t), -deltaPosition[2] * Mathf.Sin(t), Mathf.Cos(t));
                for (int i = 0; i < 4; i++) rotate[i, 1] = v[i];
                for (int i = 0; i < 4; i++)
                {
                    v = Rotate(_axis.GetColumn(i), rotate.GetColumn(0), rotate.GetColumn(1));
                    for (int j = 0; j < 4; j++) _axis[j, i] = v[j];
                }
            }

            if (nonWRotation)
            {
                if (!zRotation) { deltaRotation[0] = 0; deltaRotation[1] = 0; }
                reg1[0] = deltaRotation[0]; reg1[1] = deltaRotation[1]; reg1[2] = deltaRotation[2];
                reg2[0] = 1; reg2[1] = 0; reg2[2] = 0;
                reg2 = Vector3.ProjectOnPlane(reg2, reg1).normalized;
                if (reg2.magnitude < 0.0001) { reg2[0] = 0; reg2[1] = 1; reg2[2] = 0; }
                reg2 = Vector3.ProjectOnPlane(reg2, reg1).normalized;
                if (reg2.magnitude < 0.0001) { reg1[0] = 1; reg1[1] = 0; reg1[2] = 0; reg2[0] = 1; reg2[1] = 0; reg2[2] = 0; }
                for (int i = 0; i < 3; i++) rotate[i, 2] = reg2[i];
                v = (Quaternion.SlerpUnclamped(Quaternion.identity, deltaRotation, speedNonW * .5f) * reg2).normalized;
                for (int i = 0; i < 3; i++) rotate[i, 3] = v[i];
                for (int i = 0; i < 4; i++)
                {
                    v = Rotate(_axis.GetColumn(i), rotate.GetColumn(2), rotate.GetColumn(3));
                    for (int j = 0; j < 4; j++) _axis[j, i] = v[j];
                }
            }
        }

        public void SetVertex(Vector4 vertex)
        {
            this.vertex = vertex;
        }

        bool initializing;
        void Start()
        {
            _axis = axis;
            syncer = transform.Find("Syncer").GetComponent<Syncer>();
            var player = Networking.LocalPlayer;
            head = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            if (!inertial)
            {
                rotate[0,0] = 0; rotate[0,1] = 0; rotate[0,2] = 1; rotate[0,3] = 1;
                rotate[1,0] = 0; rotate[1,1] = 0; rotate[1,2] = 0; rotate[1,3] = 0;
                rotate[2,0] = 0; rotate[2,1] = 0; rotate[2,2] = 0; rotate[2,3] = 0;
                rotate[3,0] = 1; rotate[3,1] = 1; rotate[3,2] = 0; rotate[3,3] = 0;
            }
            else
            {
                inertialActiveW = true; inertialActiveNonW = true;
            }
            radius = 1f;
            SetFov(initFov * Mathf.PI / 180f);
            InitMesh();
            SetVertices();
            initializing = true;
            syncer.Initialize();
            initializing = false;

            domainRenderers = new MeshRenderer[domainManager.transform.childCount];
            for (int i = 0; i < domainManager.transform.childCount; i++)
            {
                Transform t = domainManager.transform.GetChild(i);
                domainRenderers[i] = t.GetComponent<MeshRenderer>();
            }
            domainCollider = domainManager.transform.GetChild(0).GetComponent<Collider>();
            foreach (MeshRenderer mr in domainRenderers) mr.enabled = false;
            domainCollider.enabled = false;
        }

        void InitMesh()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            material = GetComponent<MeshRenderer>().material;

            vertexCount = (mesh.vertexCount - 8) / 3;
            vertices = new Vector2[8][];
            centers = new Vector2[8][];
            uvs = new Vector2[8][];
            uvs[0] = mesh.uv;
            uvs[1] = mesh.uv2;
            uvs[2] = mesh.uv3;
            uvs[3] = mesh.uv4;
            uvs[4] = mesh.uv5;
            uvs[5] = mesh.uv6;
            uvs[6] = mesh.uv7;
            uvs[7] = mesh.uv8;
            for (int i = 0; i < 8; i++)
            {
                vertices[i] = new Vector2[vertexCount];
                centers[i] = new Vector2[vertexCount];
                for (int j = 0; j < vertexCount; j++)
                {
                    vertices[i][j] = uvs[i][vertexCount * 2 + j];
                    centers[i][j] = uvs[i][j];
                }
            }
        }

        void SetVertices()
        {
            if (initializing) return;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < vertexCount; j++)
                {
                    uvs[i][j] = centers[i][j] * (1 - cellSize) + vertices[i][j] * cellSize;
                    uvs[i][vertexCount + j] = centers[i][j] * edgeSize + vertices[i][j] * (1 - edgeSize);
                }
            for (int i = 0; i < 8; i++)
                mesh.SetUVs(i, uvs[i]);
        }

        void Update()
        {
            beforeUpdate = false;
            Quaternion rot = transform.rotation;
            rot[0] = 0; rot[2] = 0;
            transform.rotation = rot;
            var player = Networking.LocalPlayer;
            head = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

            if (scaleTimer > 0)
            {
                scaleTimer -= Time.deltaTime;
                if (scaleTimer <= 0) { syncer.SetScale(scaleCache); }
            }

            if (axisChanged) AxisUpdate();
            if (rotateChanged) RotateUpdate();
            Run();
            Vector4 v;
            if (inertialActiveW) for (int i = 0; i < 4; i++)
            {
                v = Rotate(_axis.GetColumn(i), rotate.GetColumn(0), rotate.GetColumn(1));
                for (int j = 0; j < 4; j++) _axis[j, i] = v[j];
            }
            if (inertialActiveNonW) for (int i = 0; i < 4; i++)
            {
                v = Rotate(_axis.GetColumn(i), rotate.GetColumn(2), rotate.GetColumn(3));
                for (int j = 0; j < 4; j++) _axis[j, i] = v[j];
            }
        }

        void Run()
        {
            material.SetFloat("_Distance", distance);
            material.SetFloat("_Tanfov", tanFov);
            material.SetFloat("_Ortho", fov == 0 ? 1 : 0);
            material.SetMatrix("_Axis", _axis);
            material.SetFloat("_Radius", radius);
            material.SetFloat("_Inversed", invertNormals ? 1 : -1);
            material.SetFloat("_Glass", glass ? 1 : 0);
            material.SetInt("_Vis", vis);
            material.SetFloat("_Border", border * 2f * transform.localScale.x);
            material.SetVector("_Vertex", vertex);
        }

        Vector4 Rotate(Vector4 src, Vector4 from, Vector4 to)
        {
            return Reflect(Reflect(src, from), (from + to).normalized);
        }

        Vector4 Reflect(Vector4 src, Vector4 normal)
        {
            return src - 2 * Vector4.Project(src, normal);
        }
    }
}