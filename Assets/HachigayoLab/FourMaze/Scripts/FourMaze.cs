
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.FourMaze
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class FourMaze : UdonSharpBehaviour
    {
        [UdonSynced] Vector4 origin = new Vector4(1.5f, 1.5f, 1.5f, 1.5f);
        [UdonSynced] Vector4 axis0 = new Vector4(1, 0, 0, 0);
        [UdonSynced] Vector4 axis1 = new Vector4(0, 1, 0, 0);
        [UdonSynced] Vector4 axis2 = new Vector4(0, 0, 1, 0);
        [UdonSynced] Vector4 axis3 = new Vector4(0, 0, 0, 1);
        [UdonSynced] int levelNum = 0;
        [UdonSynced] int owner = -1;
        [UdonSynced] bool clear;
        [UdonSynced, FieldChangeCallback(nameof(SyncChecker))] float fSyncChecker = 0;
        public float SyncChecker
        {
            get => fSyncChecker; set
            {
                ReadAxis();
                if (levelNum != currentLevelNum) NewGame(levelNum);
                else
                {
                    currentPos[0] = (int)origin[0];
                    currentPos[1] = (int)origin[1];
                    currentPos[2] = (int)origin[2];
                    currentPos[3] = (int)origin[3];
                    draw = true;
                }
                if (owner != currentOwner)
                {
                    currentOwner = owner;
                    if (currentOwner == -1)
                    {
                        playButton.interactable = true;
                        playText.text = "Play";
                    }
                    else
                    {
                        VRCPlayerApi player = VRCPlayerApi.GetPlayerById(currentOwner);
                        if (Utilities.IsValid(player)) playText.text = "Used by " + player.displayName;
                        else
                        {
                            playButton.interactable = false;
                            playText.text = "Play";
                        }

                    }
                }
                clearText.SetActive(clear);
                fSyncChecker = value;
            }
        }

        Material[] materials;
        Button playButton;
        TextMeshProUGUI timeText, playText;
        MovementController movementController;
        RotationController rotationController;
        //Score score;
        GameObject clearText, side3D, look3D, roll3D, reticle2D, reticle3D;
        Matrix4x4 axis = Matrix4x4.identity;
        int mapSize = 5, depthMax = 5, mapPosMax = 43, materialCounter, current, depth, useClip, currentLevelNum, currentOwner = -1, iMin, count, dReg, visitedCount;
        int[] backs, walls, dir, currentPos, adj, finish, oReg;
        int[][] mapPos, adjacent;
        int[][][][] map;
        bool open, request = true, draw, ready, playing, three, completion;
        bool[] visited;
        float pMin, fMin, pReg, fReg, retina = 1.8f, startTime, time;
        Vector4 vReg0, vReg1;
        Vector4[] clips;
        VRCPlayerApi localPlayer;
        int[][][] mapPosList = new int[][][]
        {
            new int[][] { new int[]{1,1,1,1}, new int[]{1,1,1,2}},
            new int[][] { new int[]{1,1,1,2}, new int[]{1,1,1,1}, new int[]{1,2,1,1}, new int[]{2,2,1,1}, new int[]{3,2,1,1}, new int[]{3,2,1,2}, new int[]{3,2,1,3}, new int[]{2,2,1,3}, new int[]{2,3,1,3}, new int[]{1,3,1,3}},
            new int[][] { new int[]{3,2,1,1}, new int[]{3,2,1,2}, new int[]{3,3,1,2}, new int[]{3,2,1,3}, new int[]{3,1,1,3}, new int[]{2,2,1,2}, new int[]{2,1,1,2}, new int[]{2,1,1,1}, new int[]{1,1,1,2}, new int[]{1,1,1,3}, new int[]{1,2,1,3}, new int[]{1,3,1,3}, new int[]{2,3,1,3}, new int[]{1,3,1,2}, new int[]{1,3,1,1}, new int[]{1,2,1,1}, new int[]{2,3,1,1}},

            new int[][] { new int[]{1,1,1,1}, new int[]{2,1,1,1}, new int[]{3,1,1,1}, new int[]{1,3,1,1}, new int[]{2,3,1,1}, new int[]{3,3,1,1}, new int[]{1,3,2,1}, new int[]{3,3,2,1}, new int[]{1,3,3,1}, new int[]{2,3,3,1}, new int[]{2,1,1,2}, new int[]{2,2,1,2}, new int[]{3,2,1,2}, new int[]{1,3,1,2}, new int[]{1,2,2,2}, new int[]{2,3,2,2}, new int[]{3,3,2,2}, new int[]{2,1,3,2}, new int[]{3,1,3,2}, new int[]{1,2,3,2}, new int[]{1,3,3,2}, new int[]{3,2,1,3}, new int[]{1,1,2,3}, new int[]{3,3,1,3}, new int[]{3,1,2,3}, new int[]{3,2,2,3}, new int[]{1,3,2,3}, new int[]{2,3,2,3}, new int[]{1,1,3,3}, new int[]{2,1,3,3}, new int[]{1,2,3,3}, new int[]{3,2,3,3}, new int[]{2,3,3,3}, new int[]{3,3,3,3}, new int[]{3,1,3,1}},

            new int[][] { new int[]{2,2,2,2},
                new int[]{3,2,2,2}, new int[]{1,2,2,2},
                new int[]{3,3,2,2}, new int[]{3,1,2,2}, new int[]{1,3,2,2}, new int[]{1,1,2,2},
                new int[]{3,3,3,2}, new int[]{3,3,1,2}, new int[]{3,1,3,2}, new int[]{3,1,1,2}, new int[]{1,3,3,2}, new int[]{1,3,1,2}, new int[]{1,1,3,2}, new int[]{1,1,1,2},
                new int[]{3,3,3,3}, new int[]{3,3,3,1}, new int[]{3,3,1,3}, new int[]{3,3,1,1}, new int[]{3,1,3,3}, new int[]{3,1,3,1}, new int[]{3,1,1,3}, new int[]{3,1,1,1}, new int[]{1,3,3,3}, new int[]{1,3,3,1}, new int[]{1,3,1,3}, new int[]{1,3,1,1}, new int[]{1,1,3,3}, new int[]{1,1,3,1}, new int[]{1,1,1,3}, new int[]{1,1,1,1},
            },

            new int[][] { new int[]{1,3,1,3}, new int[]{1,1,1,1}, new int[]{1,1,1,2}, new int[]{1,1,2,3}, new int[]{1,1,3,1}, new int[]{1,1,3,2}, new int[]{1,2,2,1}, new int[]{1,2,2,2}, new int[]{1,2,2,3}, new int[]{1,2,3,3}, new int[]{1,3,1,1}, new int[]{1,3,2,2}, new int[]{1,3,3,1}, new int[]{1,3,3,2}, new int[]{2,1,1,2}, new int[]{2,1,1,3}, new int[]{2,1,3,2}, new int[]{2,1,3,3}, new int[]{2,2,1,1}, new int[]{2,2,1,3}, new int[]{2,2,3,1}, new int[]{2,2,3,2}, new int[]{2,3,1,1}, new int[]{2,3,1,2}, new int[]{2,3,1,3}, new int[]{2,3,2,1}, new int[]{2,3,2,3}, new int[]{2,3,3,2}, new int[]{2,3,3,3}, new int[]{3,1,1,2}, new int[]{3,1,2,1}, new int[]{3,1,2,3}, new int[]{3,1,3,1}, new int[]{3,1,3,2}, new int[]{3,2,1,1}, new int[]{3,2,2,3}, new int[]{3,2,3,3}, new int[]{3,3,1,3}, new int[]{3,3,2,1}, new int[]{3,3,2,2}, new int[]{3,3,3,1}, new int[]{3,3,3,3}, new int[]{2,1,2,1}}
        };

        void Start()
        {
            localPlayer = Networking.LocalPlayer;
            Transform tf = transform.Find("Drawer");
            materials = new Material[mapPosMax];
            for (int i = 0; i < mapPosMax; i++) materials[i] = tf.GetChild(i).GetComponent<MeshRenderer>().material;
            tf = transform.Find("Play").Find("Button");
            playButton = tf.GetComponent<Button>();
            playText = tf.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            timeText = transform.Find("Timer").Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            clearText = transform.Find("Clear").gameObject;
            tf = transform.Find("Controller").Find("Movement");
            movementController = tf.GetComponent<MovementController>();
            side3D = tf.Find("Direction").Find("Direction3D").gameObject;
            tf = transform.Find("Controller").Find("Rotation");
            rotationController = tf.GetComponent<RotationController>();
            look3D = tf.Find("Direction").Find("Direction3D").gameObject;
            roll3D = tf.Find("Rotation").Find("Rotation3D").gameObject;
            reticle2D = transform.Find("Reticle2D").gameObject;
            reticle3D = transform.Find("Reticle3D").gameObject;
            //tf = transform.parent;
            //if (Utilities.IsValid(tf)) tf = tf.Find("Score");
            //if (Utilities.IsValid(tf)) score = tf.GetComponent<Score>();

            map = new int[mapSize][][][];
            for (int i = 0; i < mapSize; i++)
            {
                map[i] = new int[mapSize][][];
                for (int j = 0; j < mapSize; j++)
                {
                    map[i][j] = new int[mapSize][];
                    for (int k = 0; k < mapSize; k++)
                    {
                        map[i][j][k] = new int[mapSize];
                    }
                }
            }
            clips = new Vector4[depthMax];
            backs = new int[depthMax];
            walls = new int[depthMax];
            dir = new int[depthMax];
            currentPos = new int[4];
            oReg = new int[4];
            adj = new int[8];
            adjacent = new int[mapPosMax][];
            visited = new bool[mapPosMax];
            visited[0] = true;
            finish = new int[4];

            NewGame(3);
        }

        void NewGame(int n)
        {
            levelNum = currentLevelNum = n;
            movementController.three = rotationController.three = three = levelNum < 3;
            side3D.SetActive(!three); look3D.SetActive(!three); roll3D.SetActive(!three); reticle2D.SetActive(three); reticle3D.SetActive(!three);
            completion = levelNum >= 9;
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                    for (int k = 0; k < mapSize; k++)
                        for (int l = 0; l < mapSize; l++) map[i][j][k][l] = -1;
            for (int i = 1; i < visited.Length; i++) visited[i] = false;
            mapPos = mapPosList[n % 3 + n / 6 * 3];
            for (int i = 0; i < mapPos.Length; i++) map[mapPos[i][0]][mapPos[i][1]][mapPos[i][2]][mapPos[i][3]] = i;
            for (int i = 0; i < mapPos.Length; i++)
            {
                count = 0;
                for (int j = 0; j < 8; j++)
                {
                    currentPos[0] = mapPos[i][0]; currentPos[1] = mapPos[i][1]; currentPos[2] = mapPos[i][2]; currentPos[3] = mapPos[i][3];
                    currentPos[j / 2] += 1 - j % 2 * 2;
                    if (map[currentPos[0]][currentPos[1]][currentPos[2]][currentPos[3]] > -1) adj[count++] = j;
                }
                adjacent[i] = new int[count];
                for (int j = 0; j < count; j++) adjacent[i][j] = adj[j];
            }

            current = 0;
            currentPos[0] = mapPos[0][0];
            currentPos[1] = mapPos[0][1];
            currentPos[2] = mapPos[0][2];
            currentPos[3] = mapPos[0][3];
            if (completion) finish[0] = -1;
            else
            {
                finish[0] = mapPos[mapPos.Length - 1][0];
                finish[1] = mapPos[mapPos.Length - 1][1];
                finish[2] = mapPos[mapPos.Length - 1][2];
                finish[3] = mapPos[mapPos.Length - 1][3];
            }
            origin = new Vector4(currentPos[0] + .5f, currentPos[1] + .5f, currentPos[2] + .5f, currentPos[3] + .5f);
            axis0 = new Vector4(1, 0, 0, 0);
            axis1 = new Vector4(0, 1, 0, 0);
            axis2 = new Vector4(0, 0, 1, 0);
            axis3 = new Vector4(0, 0, 0, 1);
            axis = Matrix4x4.identity;

            request = ready = true;
            playing = false;
            time = 0;
            //if (Utilities.IsValid(score))
            //{
                //float s = score.GetScore(levelNum);
                //if (s < float.MaxValue) timeText.text = s.ToString("00.000") + "s";
                //else timeText.text = "00.000" + "s";
            //}
            /*else*/ timeText.text = "00.000" + "s";
            if (completion) timeText.text = "1/" + mapPos.Length + "\n" + timeText.text;
            clearText.SetActive(clear = false);
            backs[0] = three ? 0b110000 : 0;
            visitedCount = 1;

            materialCounter = 0;
            Build();
            for (int i = materialCounter; i < materials.Length; i++) materials[i].SetInt("_Wall", -1);
        }

        void Update()
        {
            requestCount -= Time.deltaTime;
            if (request && requestCount <= 0)
            {
                fSyncChecker = Time.realtimeSinceStartup;
                RequestSerialization();
                requestCount = .2f;
                request = false;
            }

            if (draw)
            {
                draw = false;
                if (ready) { ready = false; startTime = Time.time; playing = true; }
                materialCounter = 0;
                Build();
                for (int i = materialCounter; i < materials.Length; i++) materials[i].SetInt("_Wall", -1);
            }

            if (playing)
            {
                time = Time.time - startTime;
                timeText.text = time.ToString("00.000");
                if (completion)
                    timeText.text = visitedCount + "/" + mapPos.Length + "\n" + timeText.text;
            }
        }

        [RecursiveMethod]
        void Build()
        {
            if (current == -1) return;
            walls[depth] = backs[depth];
            foreach (int i in adjacent[current]) walls[depth] |= 1 << i;
            materials[materialCounter].SetMatrix("_Axis", axis);
            materials[materialCounter].SetVector("_ScaRet", new Vector4(.5f, .5f, three ? 0 : .5f, retina));
            materials[materialCounter].SetColor("_Color", currentPos[0] == finish[0] && currentPos[1] == finish[1] && currentPos[2] == finish[2] && currentPos[3] == finish[3] ? Color.yellow : visited[current] ? Color.gray : Color.white);
            materials[materialCounter].SetVector("_Center", new Vector4(currentPos[0] + .5f, currentPos[1] + .5f, currentPos[2] + .5f, currentPos[3] + .5f) - origin);
            materials[materialCounter].SetInt("_Wall", walls[depth]);
            materials[materialCounter].SetVectorArray("_Clips", clips);
            materials[materialCounter].SetInt("_UseClip", useClip);
            materialCounter++;

            if (depth < depthMax - 1) for (int x = 0; x < adjacent[current].Length; x++)
            {
                int i = adjacent[current][x];
                if ((backs[depth] & (1 << i)) > 0) continue;
                int a2 = i >> 1, s2 = i & 1;
                currentPos[a2] += 1 - s2 * 2;
                current = map[currentPos[0]][currentPos[1]][currentPos[2]][currentPos[3]];
                dir[depth] = i;

                if (depth > 0 && dir[depth - 1] != i)
                {
                    useClip |= 1 << depth;
                    int a1 = dir[depth - 1] >> 1, s1 = dir[depth - 1] & 1;
                    float f1 = currentPos[a1] + s1 - origin[a1];
                    float f2 = currentPos[a2] + s2 - origin[a2];
                    int s = (1 - s1 * 2) * (1 - s2 * 2);
                    Vector4 clip = Vector4.zero;
                    clip[a1] = s * f2; clip[a2] = -s * f1;
                    clips[depth] = clip;
                }
                backs[depth + 1] = backs[depth] | (1 << (i ^ 1));
                depth++;
                Build();
                depth--;
                useClip &= ~(1 << depth);
                currentPos[a2] += s2 * 2 - 1;
                current = map[currentPos[0]][currentPos[1]][currentPos[2]][currentPos[3]];
            }
        }

        float requestCount = .1f;
        Vector3 reg1 = Vector3.zero;
        Vector3 reg2 = Vector3.zero;

        public void CalcRotation(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            float t = Mathf.Sqrt(Mathf.Sqrt(2 * Mathf.PI * deltaPosition.magnitude)) * 4f * transform.localScale.x;
            deltaPosition.Normalize();
            vReg0 = new Vector4(0, 0, 0, -1);
            vReg1 = new Vector4(-deltaPosition[0] * Mathf.Sin(t), -deltaPosition[1] * Mathf.Sin(t), -deltaPosition[2] * Mathf.Sin(t), -Mathf.Cos(t));

            axis0 = Rotate(axis0, vReg0, vReg1);
            axis1 = Rotate(axis1, vReg0, vReg1);
            axis2 = Rotate(axis2, vReg0, vReg1);
            axis3 = Rotate(axis3, vReg0, vReg1);

            reg1[0] = deltaRotation[0]; reg1[1] = deltaRotation[1]; reg1[2] = deltaRotation[2];
            reg2[0] = 1; reg2[1] = 0; reg2[2] = 0;
            reg2 = Vector3.ProjectOnPlane(reg2, reg1).normalized;
            if (reg2.magnitude < 0.0001) { reg2[0] = 0; reg2[1] = 1; reg2[2] = 0; }
            reg2 = Vector3.ProjectOnPlane(reg2, reg1).normalized;
            if (reg2.magnitude < 0.0001) { reg1[0] = 1; reg1[1] = 0; reg1[2] = 0; reg2[0] = 1; reg2[1] = 0; reg2[2] = 0; }
            vReg0 = reg2;
            vReg1 = (deltaRotation * reg2).normalized;

            axis0 = Rotate(axis0, vReg1, vReg0);
            axis1 = Rotate(axis1, vReg1, vReg0);
            axis2 = Rotate(axis2, vReg1, vReg0);
            axis3 = Rotate(axis3, vReg1, vReg0);
            draw = request = true;
            ReadAxis();
        }
        void ReadAxis()
        {
            Vector4 v = axis0;
            for (int i = 0; i < 4; i++) axis[i, 0] = v[i];
            v = axis1;
            for (int i = 0; i < 4; i++) axis[i, 1] = v[i];
            v = axis2;
            for (int i = 0; i < 4; i++) axis[i, 2] = v[i];
            v = axis3;
            for (int i = 0; i < 4; i++) axis[i, 3] = v[i];
        }
        Vector4 Rotate(Vector4 src, Vector4 from, Vector4 to)
        {
            return Reflect(Reflect(src, from), (from + to).normalized);
        }
        Vector4 Reflect(Vector4 src, Vector4 normal)
        {
            return src - 2 * Vector4.Project(src, normal);
        }

        public void CalcMovement(Vector4 deltaPosition)
        {
            vReg1 = origin;
            Vector4 next = vReg1 - axis.transpose * deltaPosition;

            for (int x = 0; x < 4; x++)
            {
                iMin = 0;
                pMin = 0;
                fMin = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (vReg1[i] == next[i]) continue;
                    pReg = (int)vReg1[i];
                    pReg = (next[i] > vReg1[i]) ? pReg + 1 : (pReg == vReg1[i]) ? pReg - 1 : pReg;
                    fReg = (pReg - vReg1[i]) / (next[i] - vReg1[i]);
                    if (fReg < fMin)
                    {
                        iMin = i;
                        pMin = pReg;
                        fMin = fReg;
                    }
                }
                if (fMin == 1) break;

                vReg0 = vReg1 * (1 - fMin) + next * fMin;
                vReg0[iMin] = pMin;

                count = 0;
                for (int i = 0; i < 4; i++)
                {
                    oReg[i] = (int)vReg0[i];
                    if (oReg[i] == vReg0[i])
                    {
                        count++;
                        dReg = i;
                    }
                }

                if (count == 0) open = map[oReg[0]][oReg[1]][oReg[2]][oReg[3]] > -1;
                else if (count == 1)
                {
                    open = map[oReg[0]][oReg[1]][oReg[2]][oReg[3]] > -1;
                    oReg[dReg]--;
                    open = open && map[oReg[0]][oReg[1]][oReg[2]][oReg[3]] > -1;
                }
                else open = false;

                if (!open)
                {
                    vReg1[iMin] = next[iMin] = pMin + ((next[iMin] < pMin) ? .00001f : -.00001f);
                }
            }

            oReg[0] = (int)next[0];
            oReg[1] = (int)next[1];
            oReg[2] = (int)next[2];
            oReg[3] = (int)next[3];
            if ((iMin = map[oReg[0]][oReg[1]][oReg[2]][oReg[3]]) > -1)
            {
                current = iMin;
                if (!visited[current]) visitedCount++;
                visited[current] = true;
                currentPos[0] = oReg[0];
                currentPos[1] = oReg[1];
                currentPos[2] = oReg[2];
                currentPos[3] = oReg[3];
                if (playing && (completion ? visitedCount == mapPos.Length : (currentPos[0] == finish[0] && currentPos[1] == finish[1] && currentPos[2] == finish[2] && currentPos[3] == finish[3])))
                {
                    time = Time.time - startTime;
                    //if (Utilities.IsValid(score)) score.SetScore(levelNum, time);
                    timeText.text = time.ToString("00.000") + "s";
                    clearText.SetActive(clear = true);
                    playing = false;
                }
                origin = next;
                draw = request = true;
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (Networking.IsMaster && player.playerId == currentOwner) Quit();
        }

        public void Play()
        {
            if (!localPlayer.IsOwner(gameObject)) Networking.SetOwner(localPlayer, gameObject);
            owner = currentOwner = localPlayer.playerId;
            request = true;
        }

        public void Quit()
        {
            owner = currentOwner = -1;
            movementController.gameObject.GetComponent<VRCPickup>().Drop();
            movementController.OnDrop();
            rotationController.gameObject.GetComponent<VRCPickup>().Drop();
            rotationController.OnDrop();
            playButton.interactable = true;
            playText.text = "Play";
            NewGame(3);
        }

        public void Map1() { NewGame(0); }
        public void Map2() { NewGame(1); }
        public void Map3() { NewGame(2); }
        public void Map4() { NewGame(3); }
        public void Map5() { NewGame(4); }
        public void Map6() { NewGame(5); }
        public void Map7() { NewGame(6); }
        public void Map8() { NewGame(7); }
        public void Map9() { NewGame(8); }
        public void Map10() { NewGame(9); }
        public void Map11() { NewGame(10); }
        public void Map12() { NewGame(11); }
    }
}
