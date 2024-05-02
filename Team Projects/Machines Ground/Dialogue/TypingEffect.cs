using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace RootMain
{
    public class TypingEffect : MonoBehaviour
    {
        [SerializeField] private float charperSeconds;
        private string msg;
        private Text msgText;

        private bool nextTextOn = false;
        //private bool startTextEndTimer = false;
        //private bool nextDialog = false;
        private float textEndTimer = .0f;



        int firstIndex;    //다이얼로그에서 색상변경일때 
        int secondIndex;

        int firshtSkipIndex;
        int secondSkipIndex;

        int bFirstIndex;        //다이얼로그에서 <b>명령어있을때
        int bSecondIndex;

        int bFirshSkipIndex;
        int bSecondSkipIndex;

        int strArrayOrder = 0;

        string[] textStr;
        string[] textSkipStr;

        int idx = 0;
        string startColorStr;
        string applyColorStr;

        string bStartStr;
        string bApplyStr;

        string colorStr = "</color>";
        string stringB = "</b>";

        string startSkipColorStr;
        string applySkipColorStr;

        string bStartSkipStr;
        string bApplySkipStr;

        string colorSkipStr = "</color>";
        string stringSkipB = "</b>";

        bool colorCheck = false;
        bool colorSkipCheck = false;
        bool bCheck = false;
        bool bSkipCheck = false;        //<b>명령어 체크하는 

        int textSpacing = 0;        //텍스트 띄어쓰기를 위한 int변수
        int textSkipSpacing = 0;
        string textSkip;
        string textEff;

        int maxSpacingText = 45;
        int minSpacingText = 40;
        bool nextLineSkipcheck = false;     //다음줄로 넘어갈때 \와 n글자가 연속으로 있다면 트루로
        bool nextLineCheck = false;
        WaitForSeconds wfs;

        bool pauseCheck = false;

        //public bool textSkipCheck = false;

        //public bool NextDialog => nextDialog;
        public bool PauseCheck(bool check) => pauseCheck = check;


        Coroutine textEffectCo = null;

        public bool NextTextOn { get { return nextTextOn; }}
        public void SetMsg(string ms)
        {
            msg = ms;
            nextTextOn = false;
            //startTextEndTimer = false;
            //nextDialog = false;
            textSpacing = 0;
            colorCheck = false;
            colorSkipCheck = false;
            msgText.fontSize = 23;
            textEff = "";



            TextSplit(out textStr);     //타이핑용 텍스트배열
            TextSplit(out textSkipStr); //스킵용 텍스트배열

            if (GlobalData.language == Language.KOR)
            {
                msgText.fontSize = 30;
                //maxSpacingText = 23;
                //minSpacingText = 21;
            }
            else
            {
                msgText.fontSize = 38;
                //maxSpacingText = 39;
                //minSpacingText = 30;
            }



            strArrayOrder = 0;
            idx = 0;
            EffectStart();
        }

        void TextSplit(out string[] textString)
        {
            textString = msg.Split('<');   //<를 기준으로 string을 여러개로 쪼갬
            for (int ii = 0; ii < textString.Length; ii++)
            {
                TextInstruction("color=" ,"/color", ii , ref textString);
                TextInstruction("b>", "/b", ii, ref textString);
            }
        }

        void TextInstruction(string str1 , string str2, int idx, ref string[] textStr)
        {

            if (textStr[idx].Contains(str1))  //그안에 컬러를 지정하는 단어가 있다면 <를 붙여줌
                textStr[idx] = "<" + textStr[idx];

            if (textStr[idx].Contains(str2))  //컬러지정하는 마지막 단어가 있다면 
            {
                int index = textStr[idx].IndexOf(">");  //color> 의 마지막 깔때기의 위치를 얻고
                string strTemp = textStr[idx].Substring(index + 1);  //그 위치 다음인덱스부터 있는 string을 짤라냄
                textStr[idx] = strTemp;   //짤라낸 string을 현재 변수에 넣어줌
            }
        }

        void EffectStart()
        {
            msgText.text = "";
            textEffectCo = StartCoroutine(Effect());
        }


        IEnumerator Effect()
        {

            while(true)
            {
                if (nextTextOn)
                    yield break;



                if (textStr.Length > 1)  //만약 컬러나 b명령어가 있다면
                {
                  
                    if (strArrayOrder > textStr.Length)    //순서가 총 나눠진 배열의수를 넘어가면 끝내기
                    {
                        EffectEnd();
                        yield break;
                    }

                    if (strArrayOrder < textStr.Length) //차례대로 시작
                    {
                        if (textStr[strArrayOrder].Equals("") && strArrayOrder == 0)      //쪼갠 텍스트의 값이 비어있다면 다음으로 넘어감 
                        {
                            strArrayOrder++;

                            if (textStr[strArrayOrder].Contains("<color="))
                            {
                                colorCheck = true;
                                firstIndex = textStr[strArrayOrder].IndexOf("<color=");
                                secondIndex = textStr[strArrayOrder].IndexOf(">");
                                startColorStr = textStr[strArrayOrder].Substring(firstIndex, (secondIndex - firstIndex) + 1);
                                applyColorStr = textStr[strArrayOrder].Substring(secondIndex + 1);
                                colorStr = "</color>";
                                textStr[strArrayOrder] = applyColorStr;
                            }

                            if (textStr[strArrayOrder].Contains("<b"))
                            {
                                bCheck = true;
                                bFirstIndex = textStr[strArrayOrder].IndexOf("<b");
                                bSecondIndex = textStr[strArrayOrder].IndexOf(">");
                                bStartStr = textStr[strArrayOrder].Substring(bFirstIndex, (bSecondIndex - bFirstIndex) + 1);
                                bApplyStr = textStr[strArrayOrder].Substring(bSecondIndex + 1);
                                stringB = "</b>";
                                textStr[strArrayOrder] = bApplyStr;

                            }
                        }

                        if(bCheck)
                        {
                            if (textStr[strArrayOrder].Contains("<b"))
                            {
                                bCheck = true;
                                bFirstIndex = textStr[strArrayOrder].IndexOf("<b");
                                bSecondIndex = textStr[strArrayOrder].IndexOf(">");
                                bStartStr = textStr[strArrayOrder].Substring(bFirstIndex, (bSecondIndex - bFirstIndex) + 1);
                                bApplyStr = textStr[strArrayOrder].Substring(bSecondIndex + 1);
                                stringB = "</b>";
                                textStr[strArrayOrder] = bApplyStr;

                            }

                            textEff += bStartStr + textStr[strArrayOrder][idx] + stringB;
                            idx++;

                            //if (textSpacing < maxSpacingText)   //영어 한정 
                            //{
                            //    if (textSpacing > minSpacingText)
                            //    {


                            //        if (textStr[strArrayOrder][idx].ToString() == " ")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += "\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else if (textStr[strArrayOrder][idx].ToString() == ",")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else  //아니면 그대로 출력
                            //        {
                            //            textEff += bStartStr + textStr[strArrayOrder][idx] + stringB;
                            //            idx++;
                            //            textSpacing++;
                            //        }

                            //    }

                            //    else
                            //    {
                            //        if (textStr[strArrayOrder][idx].ToString() == " " && textSpacing == 0)
                            //        {
                            //            textEff += "";
                            //            idx++;
                            //            nextLineCheck = true;
                            //        }
                            //        else
                            //        {
                            //            textEff += bStartStr + textStr[strArrayOrder][idx] + stringB;
                            //            idx++;
                            //            textSpacing++;
                            //        }

                            //    }
                            //}
                            //else
                            //{

                            //    if (textStr[strArrayOrder][idx].ToString() == " ")
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + "";
                            //        idx++;
                            //        nextLineCheck = true;
                            //    }
                            //    else
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + bStartStr + textStr[strArrayOrder][idx] + stringB;  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //        idx++;
                            //        nextLineCheck = true;

                            //    }


                            //}

                            //msgText.text += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //idx++;
                        }



                        if (!colorCheck && !bCheck) //컬러가 없으면
                        {

                            if (idx >= textStr[strArrayOrder].Length)
                                break;

                            textEff += textStr[strArrayOrder][idx];
                            idx++;

                            //NextStringCheck(textStr, strArrayOrder, ref idx, ref nextLineCheck,ref textEff,ref textSpacing);
                            //if (textSpacing < maxSpacingText)  
                            //{
                            //    if(textSpacing > minSpacingText)
                            //    {
                            //        if (textStr[strArrayOrder][idx].ToString() == " ")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += "\n";  
                            //            idx++;
                            //            nextLineCheck = true;
                            //        }
                            //        else if (textStr[strArrayOrder][idx].ToString() == ",")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += ",\n";  
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else  //아니면 그대로 출력
                            //        {
                            //            textEff += textStr[strArrayOrder][idx];
                            //            idx++;
                            //            textSpacing++;
                            //        }

                            //    }

                            //    else
                            //    {

                            //        if (textStr[strArrayOrder][idx].ToString() == " " && textSpacing == 0)
                            //        {
                            //            textEff += "";  
                            //            idx++;
                            //            nextLineCheck = true;
                            //        }
                            //        else
                            //        {
                            //            textEff += textStr[strArrayOrder][idx];
                            //            idx++;
                            //            textSpacing++;
                            //        }


                            //    }
                            //}
                            //else
                            //{

                            //    if (textStr[strArrayOrder][idx].ToString() == " ")
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + "";
                            //        idx++;
                            //        nextLineCheck = true;
                            //    }
                            //    else
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + textStr[strArrayOrder][idx];  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //        idx++;
                            //        nextLineCheck = true;
                            //    }



                            //}


                            //msgText.text += textStr[strArrayOrder][idx];
                            //idx++;
                        }
                        else if(colorCheck && !bCheck) //컬러가 있다면
                        {
                            if (textStr[strArrayOrder].Contains("<color="))  //현재 배열에 컬러를 칭하는 문자열이 있다면 해당 문자열을 찾아서 분리후 저장 하고 적용할 문자도 분리후 저장
                            {
                                firstIndex = textStr[strArrayOrder].IndexOf("<color=");
                                secondIndex = textStr[strArrayOrder].IndexOf(">");
                                startColorStr = textStr[strArrayOrder].Substring(firstIndex, (secondIndex - firstIndex) + 1);
                                applyColorStr = textStr[strArrayOrder].Substring(secondIndex + 1);
                                colorStr = "</color>";
                                textStr[strArrayOrder] = applyColorStr;

                            }

                            textEff += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            idx++;


                            //if (textSpacing < maxSpacingText)   //영어 한정 
                            //{

                            //    if (textSpacing > minSpacingText)
                            //    {
                            //        if (textStr[strArrayOrder][idx].ToString() == " ")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += "\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else if (textStr[strArrayOrder][idx].ToString() == ",")
                            //        {
                            //            if (idx < textStr[strArrayOrder].Length)
                            //            {
                            //                if (textStr[strArrayOrder][idx + 1].ToString() == "\n")
                            //                {
                            //                    textSpacing = 0;
                            //                    textEff += ",";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                    idx++;
                            //                    nextLineCheck = true;
                            //                }
                            //                else
                            //                {
                            //                    textSpacing = 0;
                            //                    textEff += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                    idx++;
                            //                    nextLineCheck = true;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                textSpacing = 0;
                            //                textEff += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                idx++;
                            //                nextLineCheck = true;
                            //            }

         

                            //        }
                            //        else  //아니면 그대로 출력
                            //        {
                            //            textEff += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //            idx++;
                            //            textSpacing++;
                            //        }

                            //    }

                            //    else
                            //    {

                            //        if (textStr[strArrayOrder][idx].ToString() == " " && textSpacing == 0)
                            //        {
                            //            textEff += "";
                            //            idx++;
                            //            nextLineCheck = true;
                            //        }
                            //        else
                            //        {
                            //            textEff += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //            idx++;
                            //            textSpacing++;
                            //        }

                            //    }
                            //}
                            //else
                            //{

                            //    if (textStr[strArrayOrder][idx].ToString() == " ")
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + "";
                            //        idx++;
                            //        textSpacing++;
                            //        nextLineCheck = true;
                            //    }
                            //    else
                            //    {
                            //        textSpacing = 0;
                            //        textEff += "\n" + startColorStr + textStr[strArrayOrder][idx] + colorStr;  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //        idx++;
                            //        textSpacing++;
                            //        nextLineCheck = true;

                            //    }


                            //}

                            //msgText.text += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //idx++;
                        }

                        if (textStr[strArrayOrder].Length == idx)   //글자수와 인덱스가 같다면 다음으로 넘어가기
                        {
                            strArrayOrder++;                //다음으로 넘어가기위해서 플러스
                            idx = 0;                    //인덱스는 초기화
  
       
                            if(strArrayOrder == textStr.Length - 1)    //마지막문단이 빈칸이면
                            {
                                if (textStr[strArrayOrder].Length == 0)
                                    if (textStr[strArrayOrder].Contains(""))
                                        strArrayOrder++;

                                
                            }

                            if (strArrayOrder >= textStr.Length)    //순서가 총 나눠진 배열의수를 넘어가면 끝내기
                            {
                                msgText.text = textEff;
                                EffectEnd();
                                yield break;
                            }


                            if (textStr[strArrayOrder].Contains("<color="))
                            {
                                colorCheck = true;
                                firstIndex = textStr[strArrayOrder].IndexOf("<color=");
                                secondIndex = textStr[strArrayOrder].IndexOf(">");
                                startColorStr = textStr[strArrayOrder].Substring(firstIndex, (secondIndex - firstIndex) + 1);
                                applyColorStr = textStr[strArrayOrder].Substring(secondIndex + 1);
                                colorStr = "</color>";
                                textStr[strArrayOrder] = applyColorStr;
                            }
                            else
                                colorCheck = false;


                            if (textStr[strArrayOrder].Contains("<b"))
                            {
                                bCheck = true;
                                bFirstIndex = textStr[strArrayOrder].IndexOf("<b");
                                bSecondIndex = textStr[strArrayOrder].IndexOf(">");
                                bStartStr = textStr[strArrayOrder].Substring(bFirstIndex, (bSecondIndex - bFirstIndex) + 1);
                                bApplyStr = textStr[strArrayOrder].Substring(bSecondIndex + 1);
                                stringB = "</b>";
                                textStr[strArrayOrder] = bApplyStr;

                            }
                            else
                                bCheck = false;
 
                        }

                    }


                }

                else    //기본적인 다이얼로그라면 있는 그대로 출력
                {
                    if (idx > textStr[strArrayOrder].Length)
                        break;


                    textEff += textStr[strArrayOrder][idx];
                    idx++;

                    //NextStringCheck(textStr, strArrayOrder, ref idx, ref nextLineCheck, ref textEff, ref textSpacing);


                    //if (textSpacing < maxSpacingText)   //영어 한정 
                    //{
                    //    if (idx > textStr[strArrayOrder].Length)
                    //        yield break;

                    //    if (textSpacing > minSpacingText)
                    //    {
                    //        if (textStr[strArrayOrder][idx].ToString() == " ")
                    //        {
                    //            textSpacing = 0;
                    //            textEff += "\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                    //            idx++;
                    //            nextLineCheck = true;


                    //        }
                    //        else if (textStr[strArrayOrder][idx].ToString() == ",")
                    //        {
                    //            if(idx < textStr[strArrayOrder].Length)
                    //            {
                    //                if (textStr[strArrayOrder][idx + 1].ToString() == "\n")
                    //                {
                    //                    textSpacing = 0;
                    //                    textEff += ",";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                    //                    idx++;
                    //                    nextLineCheck = true;
                    //                }
                    //                else
                    //                {
                    //                    textSpacing = 0;
                    //                    textEff += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                    //                    idx++;
                    //                    nextLineCheck = true;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                textSpacing = 0;
                    //                textEff += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                    //                idx++;
                    //                nextLineCheck = true;
                    //            }


                    //        }
                    //        else  //아니면 그대로 출력
                    //        { 
                    //            textEff += textStr[strArrayOrder][idx];
                    //            idx++;
                    //            textSpacing++;


                    //        }

                    //    }

                    //    else
                    //    {
                    //        if (textStr[strArrayOrder][idx].ToString() == " " && textSpacing == 0)
                    //        {
                    //            textEff += "";
                    //            idx++;
                    //            nextLineCheck = true;
                    //        }
                    //        else
                    //        {
                    //            textEff += textStr[strArrayOrder][idx];
                    //            idx++;
                    //            textSpacing++;

                    //        }

                    //    }
                    //}
                    //else
                    //{
                    //    if (idx > textStr[strArrayOrder].Length)
                    //        yield break;

                    //    if (textStr[strArrayOrder][idx].ToString() == " ")
                    //    {
                    //        textSpacing = 0;
                    //        textEff += "\n" + "";
                    //        idx++;
                    //        nextLineCheck = true;
                    //    }
                    //    else
                    //    {
                    //        textSpacing = 0;
                    //        textEff += "\n" + textStr[strArrayOrder][idx];
                    //        idx++;
                    //        nextLineCheck = true;
                    //    }



                    //}

                    if (textStr[strArrayOrder].Length == idx)   //글자수와 인덱스가 같다면 다음으로 넘어가기
                    {
                        idx = 0;                    //인덱스는 초기화
                        strArrayOrder++;

                        if (strArrayOrder >= textStr.Length)    //순서가 총 나눠진 배열의수를 넘어가면 끝내기
                        {
                            msgText.text = textEff;
                            EffectEnd();
                            yield break;
                        }


                    }

                }

                msgText.text = textEff;

                yield return wfs;
            }
           
        }
        
        

        void EffectEnd()
        {
            nextTextOn = true;
            startColorStr = "";
        }

    // Start is called before the first frame update
    void Awake()
        {
            msgText = GetComponent<Text>();
            wfs = new WaitForSeconds(1 / charperSeconds);

        }

        // Update is called once per frame
        void Update()
        {
            if ((!nextTextOn && pauseCheck) || (!nextTextOn && SceneManager.GetActiveScene().name.Contains("Lobby_Scene")))
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetMouseButtonDown(0))       //스페이스를 누르면 스크립트가 전부 표시
                {

                    if (textEffectCo != null)
                        StopCoroutine(textEffectCo);


                    msgText.text = "";
                    textSkip = "";
                    textSkipSpacing = 0;
                    idx = 0;
                    strArrayOrder = 0;
                    colorSkipCheck = false;
                   
                    if (textSkipStr.Length <= 1)  //색깔없는 스트링일때 (1개)
                    {

                        for (int ii = 0; ii < textSkipStr[strArrayOrder].Length; ii++)  //현재 스트링의 길이만큼 반복
                        {
                            if (idx >= textSkipStr[strArrayOrder].Length)
                                break;

                            textSkip += textSkipStr[strArrayOrder][idx];
                            idx++;

                            //NextStringCheck(textSkipStr, strArrayOrder,ref idx,ref nextLineSkipcheck,ref textSkip, ref textSkipSpacing);

                            //if (textSkipSpacing < maxSpacingText)
                            //{
                            //    if (textSkipSpacing > minSpacingText)  //34번째 글자수보다 컷을때 띄어쓰기가 있다면
                            //    {
                            //        if (textSkipStr[strArrayOrder][idx].ToString() == " ") //띄어쓰기가 있다면 넘겨버리기
                            //        {
                            //            textSkipSpacing = 0;
                            //            textSkip += "\n";  
                            //            idx++;
                            //            nextLineSkipcheck = true;
                            //        }
                            //        else if (textSkipStr[strArrayOrder][idx].ToString() == ",")
                            //        {

                            //            if (idx < textSkipStr[strArrayOrder].Length)
                            //            {
                            //                if (textSkipStr[strArrayOrder][idx + 1].ToString() == "\n")
                            //                {
                            //                    textSkipSpacing = 0;
                            //                    textSkip += ",";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                    idx++;
                            //                    nextLineSkipcheck = true;
                            //                }
                            //                else
                            //                {
                            //                    textSkipSpacing = 0;
                            //                    textSkip += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                    idx++;
                            //                    nextLineSkipcheck = true;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                textSpacing = 0;
                            //                textSkip += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //                idx++;
                            //                nextLineSkipcheck = true;
                            //            }

                            //        }
                            //        else  //아니면 그대로 출력
                            //        {
                            //            textSkip += textSkipStr[strArrayOrder][idx];
                            //            idx++;
                            //            textSkipSpacing++;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (textSkipStr[strArrayOrder][idx].ToString() == " " && textSkipSpacing == 0)
                            //        {
                            //            textSkip += "";
                            //            idx++;
                            //            nextLineCheck = true;
                            //        }

                            //        else
                            //        {
                            //            textSkip += textSkipStr[strArrayOrder][idx];
                            //            idx++;
                            //            textSkipSpacing++;
                            //        }

                            //    }

                            //}
                            //else //textSkipSpacing이 max치를 넘으면
                            //{
                            //    if (textSkipStr[strArrayOrder][idx].ToString() == " ")
                            //    {
                            //        //띄어쓰기가 있다면 다음칸으로 넘어가고 공백제거
                            //        textSkipSpacing = 0;
                            //        textSkip += "\n" + "";
                            //        idx++;
                            //        nextLineSkipcheck = true;

                            //    }
                            //    else //띄어쓰기가 없다면 다음칸으로 넘어가면서 다음글자 찍기
                            //    {
                            //        textSkipSpacing = 0;
                            //        textSkip += "\n" + textSkipStr[strArrayOrder][idx];  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                            //        idx++;
                            //        nextLineSkipcheck = true;

                            //    }

                            //}
                        }
                    }

                    else    //색깔이있는 스트링일때 (여러 배열로 나눠짐)
                    {
                        for(int ii = 0; ii < textSkipStr.Length; ii++)
                        {
                            if (textSkipStr[ii].Equals(""))
                                continue;

                            colorSkipCheck = false;
                            bSkipCheck = false;
                            for(int jj = 0; jj < textSkipStr[ii].Length; jj++)
                            {
                                if (!colorSkipCheck)
                                {
                                    if (textSkipStr[ii].Contains("<color="))  //다음 배열에 컬러를 칭하는 문자열이 있다면 해당 문자열을 찾아서 분리후 저장 하고 적용할 문자도 분리후 저장
                                    {
                                        colorSkipCheck = true;
                                        firshtSkipIndex = textSkipStr[ii].IndexOf("<color=");
                                        secondSkipIndex = textSkipStr[ii].IndexOf(">");
                                        startSkipColorStr = textSkipStr[ii].Substring(firshtSkipIndex, (secondSkipIndex - firshtSkipIndex) + 1);
                                        applySkipColorStr = textSkipStr[ii].Substring(secondSkipIndex + 1);
                                        colorSkipStr = "</color>";
                                        textSkipStr[ii] = applySkipColorStr;

                                    }
                                    else
                                    {
                                        
                                        startSkipColorStr = "";
                                        colorSkipStr = "";
                                    }
                                }

                                if(!bSkipCheck)
                                {
                                    if (textSkipStr[ii].Contains("<b"))
                                    {
                                        bSkipCheck = true;
                                        bFirshSkipIndex = textSkipStr[ii].IndexOf("<b");
                                        bSecondSkipIndex = textSkipStr[ii].IndexOf(">");
                                        bStartSkipStr = textSkipStr[ii].Substring(bFirshSkipIndex, (bSecondSkipIndex - bFirshSkipIndex) + 1);
                                        bApplySkipStr = textSkipStr[ii].Substring(bSecondSkipIndex + 1);
                                        stringSkipB = "</b>";
                                        textSkipStr[ii] = bApplySkipStr;

                                    }
                                    else
                                    {
                                        bStartSkipStr = "";
                                        stringSkipB = "";
                                    }
                                }

                                if (idx >= textSkipStr[ii].Length)
                                    break;


                                if (colorSkipCheck && !bSkipCheck)
                                {
                                    textSkip += startSkipColorStr + textSkipStr[ii][idx] + colorSkipStr;
                                    idx++;

                                }

                                else if (bSkipCheck && !colorSkipCheck)
                                {
                                    textSkip += bStartSkipStr + textSkipStr[ii][idx] + stringSkipB;
                                    idx++;
                                }
                                else
                                {
                                    textSkip += textSkipStr[ii][idx];
                                    idx++;
                                }


                                //NextStringCheck(textSkipStr, ii, ref idx, ref nextLineSkipcheck, ref textSkip, ref textSkipSpacing);


                                //if (textSkipSpacing < maxSpacingText)
                                //{
                                //    if (textSkipSpacing == 0 && idx == 0 && textSkipStr[ii][idx].ToString() == " ")
                                //    {
                                //        //인덱스가 0일때 띄어쓰기가 있다면
                                //        textSkip += "";     //빈칸
                                //        idx++;
                                //        textSkipSpacing++;
                                //    }


                                //    if(textSkipSpacing > minSpacingText)  //40번째 글자수보다 컷을때 띄어쓰기가 있다면
                                //    {
                                //        if (textSkipStr[ii][idx].ToString() == " ") //띄어쓰기가 있다면 넘겨버리기
                                //        {
                                //            textSkipSpacing = 0;
                                //            textSkip += "\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                                //            idx++;
                                //            nextLineSkipcheck = true;

                                //        }
                                //        else if (textSkipStr[ii][idx].ToString() == ",")
                                //        {
                                //            if (idx < textSkipStr[strArrayOrder].Length)
                                //            {
                                //                if (textSkipStr[strArrayOrder][idx + 1].ToString() == "\n")
                                //                {
                                //                    textSkipSpacing = 0;
                                //                    textSkip += ",";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                                //                    idx++;
                                //                    nextLineSkipcheck = true;
                                //                }
                                //                else
                                //                {
                                //                    textSkipSpacing = 0;
                                //                    textSkip += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                                //                    idx++;
                                //                    nextLineSkipcheck = true;
                                //                }
                                //            }
                                //            else
                                //            {
                                //                textSpacing = 0;
                                //                textSkip += ",\n";  //이거하니깐 처음에 msg와 text값이 안비슷해져가지고 스킵이 안댐(고쳐야함)
                                //                idx++;
                                //                nextLineSkipcheck = true;
                                //            }


                                //        }
                                //        else  //아니면 그대로 출력
                                //        {
                                //            if(colorSkipCheck && !bSkipCheck)
                                //            {
                                //                textSkip += startSkipColorStr + textSkipStr[ii][idx] + colorSkipStr;
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }

                                //            else if(bSkipCheck && !colorSkipCheck)
                                //            {
                                //                textSkip += bStartSkipStr + textSkipStr[ii][idx] + stringSkipB;
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }
                                //            else
                                //            {
                                //                textSkip += textSkipStr[ii][idx];
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }

                                //        }
                                //    }
                                //    else
                                //    {
                                //        if (textSkipStr[ii][idx].ToString() == " " && textSkipSpacing == 0)
                                //        {
                                //            textSkip += "";
                                //            idx++;
                                //            nextLineSkipcheck = true;
                                //        }
                                //        else
                                //        {
                                //            if (colorSkipCheck && !bSkipCheck)
                                //            {
                                //                textSkip += startSkipColorStr + textSkipStr[ii][idx] + colorSkipStr;
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }

                                //            else if (bSkipCheck && !colorSkipCheck)
                                //            {
                                //                textSkip += bStartSkipStr + textSkipStr[ii][idx] + stringSkipB;
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }
                                //            else
                                //            {
                                //                textSkip += textSkipStr[ii][idx];
                                //                idx++;
                                //                textSkipSpacing++;
                                //            }
                                //        }

                                //    }


                                //}
                                //else
                                //{
                                //    if (textSkipStr[ii][idx].ToString() == " ")
                                //    {
                                //        //띄어쓰기가 있다면 다음칸으로 넘어가고 공백제거
                                //        textSkipSpacing = 0;
                                //        textSkip += "\n" + "";
                                //        idx++;
                                //        textSkipSpacing++;
                                //        nextLineSkipcheck = true;

                                //    }
                                //    else //띄어쓰기가 없다면 다음칸으로 넘어가면서 다음글자 찍기
                                //    {

                                //        if (colorSkipCheck && !bSkipCheck)
                                //        {
                                //            textSkipSpacing = 0;
                                //            textSkip += "\n" + startSkipColorStr + textSkipStr[ii][idx] + colorSkipStr;
                                //            idx++;
                                //            textSkipSpacing++;

                                //            nextLineSkipcheck = true;

                                //        }

                                //        else if (bSkipCheck && !colorSkipCheck)
                                //        {
                                //            textSkipSpacing = 0;
                                //            textSkip += "\n" + bStartSkipStr + textSkipStr[ii][idx] + stringSkipB;
                                //            idx++;
                                //            textSkipSpacing++;

                                //            nextLineSkipcheck = true;

                                //        }
                                //        else
                                //        {
                                //            textSkipSpacing = 0;
                                //            textSkip += "\n" + textSkipStr[ii][idx];
                                //            idx++;
                                //            textSkipSpacing++;

                                //            nextLineSkipcheck = true;

                                //        }


                                //    }


                                //}

                                if (textSkipStr[ii].Length == idx)   //글자수와 인덱스가 같다면 다음으로 넘어가기
                                {
                                    idx = 0;                    //인덱스는 초기화

                                    if (ii + 1 >= textSkipStr.Length)    //순서가 총 나눠진 배열의수를 넘어가면 끝내기
                                    {
                                        msgText.text = textSkip;
                                        EffectEnd();

                                        return;
                                    }


        


                                }
                            }
                        }
                         //색깔있는거는 다시 스트링값을 새로 받아서 전체로 표시


                    }
                    


                    msgText.text = textSkip;
                    EffectEnd();
                }

            }







        }


        void NextStringCheck(string[] str , int strArrayidx , ref int index, ref bool check, ref string textStr, ref int textSpacing)
        {
            //check가 트루로 들어오면 다음줄로 이동했다는 것 
            //여기서 다음줄로 넘어왔을때 \n을 체크하고 없애준다.

            if (str[strArrayidx][str[strArrayidx].Length - 1].ToString() == "\n") //\n이 마지막글자일때
            {
                textSpacing = 0;
                return;

            }


            if (check)  //혹시 바꿔야할지 모르기때문에 true false 둘다 똑같이 놔둠
            {
                if (str[strArrayidx][index].ToString() == "\n")  //true일때  check변수만 빼고 나머지 다 주석처리하면 첫째줄에서 띄어쓰기가 없을때 다음줄에서 \n가 작동안함
                {
                    textStr += str[strArrayidx][index];
                    textSpacing = 0;
                    index++;
                    check = false;
                }
            }
            else
            {
                if (str[strArrayidx][index].ToString() == "\n")
                {
                    textStr += str[strArrayidx][index];
                    textSpacing = 0;
                    index++;
                }
            }
        }

    }
}
