using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace cloudEXP
{
    public class SchProblem : MonoBehaviour
    {
        private BaseGameManager bgmi
        {
            get
            {
                return Singleton<BaseGameManager>.Instance;
            }
        }
        private EnvironmentController ec
        {
            get
            {
                return Singleton<BaseGameManager>.Instance.Ec;
            }
        }
        public static SchProblem? Instance { get; private set; }
        public void Initialize()
        {
            schstate = 0;
            playedgetout = false;
            bfalpha = 0f;
            bpos = 315f;
            introtime = 0;
            speed = 0;
        }
        public void Awake()
        {
            SchProblem.Instance = this;
            schproblemblackbartop = new GameObject().AddComponent<Image>();
            schproblemblackbarbottom = new GameObject().AddComponent<Image>();
            schproblemflash = new GameObject().AddComponent<Image>();

            schproblemblackbartop.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform);
            schproblemblackbarbottom.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform);
            schproblemflash.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform);

            schproblemblackbartop.color = Color.black;
            schproblemblackbarbottom.color = Color.black;
            schproblemflash.color = new Color(1f, 1f, 1f, 0f);

            schproblemblackbarbottom.rectTransform.localScale = new Vector3(10f, 1.5f, 1f);
            schproblemblackbartop.rectTransform.localScale = new Vector3(10f, 1.5f, 1f);
            schproblemflash.rectTransform.localScale = new Vector3(10f, 10f, 2f);

            schproblemblackbartop.rectTransform.anchoredPosition3D = Vector3.zero;
            schproblemblackbarbottom.rectTransform.anchoredPosition3D = Vector3.zero;
            schproblemflash.rectTransform.anchoredPosition3D = Vector3.zero;
        }
        public void OnDestroy()
        {
            introtime = 0;
            Singleton<MusicManager>.Instance.StopFile();
        }
        public void Update()
        {
            int exitsfound = ec.ElevatorManager.Elevators.Count((Elevator x) => x.CurrentState == ElevatorState.OutOfOrder);
            if (schproblemblackbarbottom != null)
            {
                if (schstate == 1 && bpos > 175f)
                {
                    bpos -= (speed + 1) * Time.deltaTime * 11.2f;//25
                }
                if (schstate == 2 && bpos < 315f)
                {
                    bpos += (315f - bpos) / 8;
                }
                if (schproblemblackbartop.rectTransform.anchoredPosition3D.y != bpos)
                {
                    schproblemblackbartop.rectTransform.anchoredPosition3D = new Vector3(0f, bpos, 0f);
                    schproblemblackbarbottom.rectTransform.anchoredPosition3D = new Vector3(0f, -bpos, 0f);
                }
                schproblemflash.gameObject.SetActive(bfalpha > 0f);
                if (bfalpha > 0f)
                {
                    bfalpha -= Time.deltaTime;
                    schproblemflash.color = new Color(1f, 1f, 1f, bfalpha);
                }
                schproblemblackbartop.gameObject.SetActive(schstate > 0);
                schproblemblackbarbottom.gameObject.SetActive(schstate > 0);
                if (ec != null && schstate == 0 && (bgmi.FoundNotebooks > 0 || exitsfound > 0) && bgmi.AllNotebooksFound)
                {
                    bool secret = (Mathf.FloorToInt(Random.Range(1, 99)) == 1);
                    Singleton<MusicManager>.Instance.StopMidi();
                    darktemp = ec.standardDarkLevel;
                    if (!secret)
                    {
                        ec.standardDarkLevel = Color.black;
                        ec.InitializeLighting();
                    }
                    schstate = 0;
                    Singleton<MusicManager>.Instance.PlayMidi(secret ? "CampMinigame_1_1" : "custom_schproblem1", secret);
                    schstate = secret ? 2 : 1;
                    bpos = 315f;
                }
                if (schstate > 0)
                {
                    if (exitsfound > 0)
                    {
                        if (speed < exitsfound * 0.25f)
                        {
                            speed += Time.deltaTime / 2;
                            Singleton<MusicManager>.Instance.SetSpeed(1f + speed);
                        }
                        if (!playedgetout && ec != null && ec.ElevatorManager.ExitAvailable)
                        {
                            // put audio code here :3
                            playedgetout = true;
                        }
                    }
                    if (schstate == 1)
                    {
                        introtime += Time.deltaTime * (1f + speed);
                        if (introtime >= 17.7f)
                        {
                            schstate = 0;
                            Singleton<MusicManager>.Instance.PlayMidi("custom_schproblem2", true);
                            schstate = 2;
                            Singleton<MusicManager>.Instance.SetSpeed(1f + speed);
                            if (ec != null)
                            {
                                ec.standardDarkLevel = darktemp;
                                ec.SetAllLights(true);
                                ec.InitializeLighting();
                                foreach (Cell cell in ec.cells)
                                {
                                    if (cell.lightStrength > 0)
                                    {
                                        cell.lightColor = Color.red;
                                        ec.RegenerateLight(cell);
                                    }
                                }
                            }
                            schproblemflash.color = Color.white;
                            bfalpha = 1f;
                            introtime = 0f;
                            Shader.SetGlobalColor("_SkyboxColor", Color.red);
                        }
                    }
                    if (bgmi.NotebookTotal >= 9 && Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Transpose < 2)
                    {
                        Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Transpose = 2;
                    }
                }
            }
        }
        private Image? schproblemblackbartop;
        private Image? schproblemblackbarbottom;
        private Image? schproblemflash;
        private bool playedgetout = false;
        public int schstate = 0;
        public float bfalpha = 0f;
        private float bpos = 0f;
        private float introtime = 0;
        private float speed = 0;
        private Color darktemp = Color.black;
    }
}