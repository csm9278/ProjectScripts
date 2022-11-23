using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using Altair;

//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;
//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트

public class Title_Mgr : MonoBehaviour
{
    [Header("LoginPanel")]              //이렇게 쓰면 편집창에 태그들이 나온다. 
    public GameObject m_LoginPanelObj;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;
    public InputField IDInputField;     //Email 로 받을 것임
    public InputField PassInputField;

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanelObj;
    public InputField New_IDInputField;  //Email 로 받을 것임
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CreateAccountBtn = null;
    public Button m_CancelButton = null;

    [Header("Normal")]
    public Text MessageText;
    float showMsTimer = 0.0f;

    bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크
    bool loginCheck = false;             //로그인이 성공인지 체크
    bool bringLoginPanel = false;       //로그인 판넬을 가지고 갓는지?

    public GameObject RobotArms;
    public Animation anim;
    Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    string loginUrl = "";
    string createUrl = "";

    string keyStr = "";

    [Header("----Scene Move----")]
    [SerializeField] private string lobbySceneName;

    // Start is called before the first frame update
    void Start()
    {

        //------- LoginPanel
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);

        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //------- CreateAccountPanel
        if (m_CancelButton != null)
            m_CancelButton.onClick.AddListener(CreateCancelBtn);

        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

        GlobalData.SaveInitData();

        loginUrl = "http://pmaker.dothome.co.kr/GreateTeam/PvZLogin.php";
        createUrl = "http://pmaker.dothome.co.kr/GreateTeam/PvZCreateAccount.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < showMsTimer)
        {
            showMsTimer -= Time.deltaTime;
            if (showMsTimer <= 0.0f)
            {
                MessageOnOff("", false); //메시지 끄기
            }
        }

        if (loginCheck)
        {
            RobotArms.transform.Translate(new Vector3(0.0f, -1.0f, 0.0f) * Time.deltaTime * 25.0f);
            Vector3 pos = RobotArms.transform.position;

            if (RobotArms.transform.position.x != 0.0f)
            {
                pos.x = 0.0f;
                pos.z = 2.31f;
                RobotArms.transform.position = pos;
            }

            if (pos.y <= 7.71f)
            {
                loginCheck = false;
                anim.Play();
                StartCoroutine(BringPanel());

            }
        }

        if (bringLoginPanel)
        {
            RobotArms.transform.Translate(new Vector3(0.0f, 1.0f, 0.0f) * Time.deltaTime * 50.0f);
            Vector3 pos = RobotArms.transform.position;

            if (RobotArms.transform.position.x != 0.0f)
            {
                pos.x = 0.0f;
                pos.z = 2.31f;
                RobotArms.transform.position = pos;
            }


            if (m_LoginPanelObj.transform.localPosition.y >= 700.0f)
            {
                bringLoginPanel = false;
                SceneManager.LoadScene(lobbySceneName);
                return;

            }

            m_LoginPanelObj.transform.Translate(Vector3.up * Time.deltaTime * 10.0f);

        }

        TabFunc();
    }

    IEnumerator BringPanel()
    {
        yield return new WaitForSeconds(0.7f);
        bringLoginPanel= true;
    }

    void TabFunc()
	{
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //로그인 판넬
            if (IDInputField.isFocused == true)
            {
                PassInputField.Select();
            }

            //회원가입 판넬
            if (New_IDInputField.isFocused == true)
            {
                New_PassInputField.Select();
            }
            if (New_PassInputField.isFocused == true)
            {
                New_NickInputField.Select();
            }
        }
    }

    void LoginBtn()
    {
        string a_IdStr = IDInputField.text;
        string a_PwStr = PassInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();

        if ( string.IsNullOrEmpty(a_IdStr) == true ||
             string.IsNullOrEmpty(a_PwStr) == true )
        {
            MessageOnOff("ID, PW 빈칸 없이 입력해 주셔야 합니다.");
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
        {
            MessageOnOff("ID는 3글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
        {
            MessageOnOff("비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine(LoginCo(a_IdStr, a_PwStr));
    }

    IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        //GlobalData.choi_InitData();
        if (GlobalData.choi_m_TrList.Count <= 0)           //처음 글로벌 유저데이터리스트가 없다면 초기화갱신
        {
            GlobalData.choi_InitData();                    //전체 터렛의 데이터 초기화
            Yuspace.GlobalValue.InitData();                         //상점을 위한 터렛타입 초기화ㅣ
        }

        for (int ii = 0; ii < 5; ii++)
        {
            Debug.Log(GlobalData.choi_m_TrList[ii].m_name);
        }

        Debug.Log(GlobalData.choi_TurretNameList.Length);


        WWWForm form = new WWWForm();
        form.AddField("Account", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Password", a_PwStr);

        UnityWebRequest a_www = UnityWebRequest.Post(loginUrl, form);
        yield return a_www.SendWebRequest();    // 응답이 올때까지 대기하기...

        if(a_www.error == null) //에러가 나지 않았을때
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if(sz.Contains("Login-Success!!") == false)
            {
                if(sz.Contains("ID does not exist") == true)
                    MessageOnOff("아이디가 존재하지 않습니다.");
                else if(sz.Contains("Password does not match") == true)
                    MessageOnOff("비밀번호가 일치하지 않습니다.");
                else
                    MessageOnOff("로그인 실패, 다시 시도해 주세요." + sz);

                yield break;
            }

            if(sz.Contains("{\"") == false)
            {
                MessageOnOff("서버의 응답이 정상적이지 않습니다. : " + sz);
                yield break;
            }

            GlobalData.choi_UniqueID = a_IdStr;

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));

            //JSON 파싱
            var N = JSON.Parse(a_GetStr);
            if (N == null)
                yield break;

            if (N["nick_name"] != null)
                GlobalData.choi_userNick = N["NickName"];

            for (int ii = 1; ii < 4; ii++)  //터렛 && 스테이지 데이터
			{
                keyStr = "SlotData" + ii.ToString();
                if (N[keyStr] != null)
				{
                    string m_StrJson = N[keyStr];

                    if (string.IsNullOrEmpty(m_StrJson) == false &&
                    m_StrJson.Contains("TrList") == true)
                    {
                        //--myInfo 쪽 JSON 파일 파싱
                        var NN = JSON.Parse(m_StrJson);
                        Debug.Log(NN);
                        for (int jj = 0; jj < NN["TrList"].Count; jj++)
                        {
                            int a_CrLevel = NN["TrList"][jj].AsInt;

                            Debug.Log("테렛");
                            if (jj < GlobalData.choi_m_TrList.Count)
                            {
                                GlobalData.choi_m_SaveTrList[ii, jj] = a_CrLevel;
                                Debug.Log(GlobalData.choi_m_SaveTrList[ii, jj]);
                            }
                        }
                        int a_Stage = NN["Stage"].AsInt;
                        GlobalData.choi_StageList[ii] = a_Stage;
                        Debug.Log(a_Stage);
                    }
                    else
                    {
                        for (int jj = 0; jj < GlobalData.choi_m_TrList.Count; jj++)
                        {
                            if (jj < GlobalData.choi_m_TrList.Count)
                            {
                                GlobalData.choi_m_SaveTrList[ii, jj] = 0;
                                Debug.Log(GlobalData.choi_m_SaveTrList[ii, jj]);
                            }
                        }
                        GlobalData.choi_StageList[ii] = 0;
                    }
                }

			}
            
            for (int ii = 1; ii < 4; ii++)  //터렛 && 스테이지 데이터
			{
                keyStr = "Diamond" + ii.ToString();
				if (N[keyStr] != null)
				{
                    int a_Diamonds = N[keyStr].AsInt;
                    GlobalData.choi_DiamondList[ii] = a_Diamonds;
                }

			}

			//SceneManager.LoadScene("LobbyScene");
			Debug.Log("로그인 성공");

            loginCheck = true;


        }
        else
        {
            Debug.Log(a_www.error.ToString());
        }
    }
    public void OpenCreateAccBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(false);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(true);

        m_CreateAccountBtn.image.color = color;     //계정생성을 누르면 색깔을 흐리게
    }

    public void CreateCancelBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(true);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(false);
    }

    public void CreateAccountBtn() //계정 생성 요청 함수
    {
        string a_IdStr = New_IDInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();
        a_NickStr = a_NickStr.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
            string.IsNullOrEmpty(a_PwStr) == true ||
            string.IsNullOrEmpty(a_NickStr) == true)
        {
            MessageOnOff("ID, PW, 별명 빈칸 없이 입력해 주셔야 합니다.");
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
        {
            MessageOnOff("ID는 3글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
        {
            MessageOnOff("비밀번호는 6글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length < 20))  //2~20
        {
            MessageOnOff("별명은 2글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
        //StartCoroutine(CreateCo(a_IdStr, a_PwStr));
    } //public void CreateAccountBtn()

    IEnumerator CreateCo(string a_IDStr, string a_PWStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Account", a_IDStr, System.Text.Encoding.UTF8);
        form.AddField("Password", a_PWStr);
        form.AddField("NickName", a_NickStr, System.Text.Encoding.UTF8);
        //웹서버에서 받을 때 한글이 안깨지게 하려면
        //System.Text.Encoding.UTF8를 추가해줘야한다.

        UnityWebRequest a_www = UnityWebRequest.Post(createUrl, form);
        yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...
        
        if(a_www.error == null) //에러가 없을 때
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            if (sz.Contains("Create Success.") == true)
                MessageOnOff("가입 성공");
            else if (sz.Contains("ID does exist.") == true)
                MessageOnOff("중복된 ID가 존재합니다.");
            else if (sz.Contains("NickName does exist.") == true)
                MessageOnOff("중복된 닉네임이 존재합니다.");
            else
                MessageOnOff(sz);


            m_CreateAccOpenBtn.image.color = color;     //계정생성하면 버튼색상 다시 흐리게
        }
        else
        {
            MessageOnOff("가입실패" + a_www.error);
            Debug.Log(a_www.error);
        }
    }

    void MessageOnOff(string Mess = "", bool isOn = true)
    {
        if (isOn == true)
        {
            MessageText.text = Mess;
            MessageText.gameObject.SetActive(true);
            showMsTimer = 7.0f;
        }
        else
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }




    //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
    //https://blog.naver.com/rlawndks4204/221591566567
    // <summary>
    /// 올바른 이메일인지 체크.
    /// </summary>
    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    /// <summary>
    /// 도메인으로 변경해줌.
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }
    //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트

}
