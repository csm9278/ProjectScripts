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



        int firstIndex;    //���̾�α׿��� ���󺯰��϶� 
        int secondIndex;

        int firshtSkipIndex;
        int secondSkipIndex;

        int bFirstIndex;        //���̾�α׿��� <b>��ɾ�������
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
        bool bSkipCheck = false;        //<b>��ɾ� üũ�ϴ� 

        int textSpacing = 0;        //�ؽ�Ʈ ���⸦ ���� int����
        int textSkipSpacing = 0;
        string textSkip;
        string textEff;

        int maxSpacingText = 45;
        int minSpacingText = 40;
        bool nextLineSkipcheck = false;     //�����ٷ� �Ѿ�� \�� n���ڰ� �������� �ִٸ� Ʈ���
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



            TextSplit(out textStr);     //Ÿ���ο� �ؽ�Ʈ�迭
            TextSplit(out textSkipStr); //��ŵ�� �ؽ�Ʈ�迭

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
            textString = msg.Split('<');   //<�� �������� string�� �������� �ɰ�
            for (int ii = 0; ii < textString.Length; ii++)
            {
                TextInstruction("color=" ,"/color", ii , ref textString);
                TextInstruction("b>", "/b", ii, ref textString);
            }
        }

        void TextInstruction(string str1 , string str2, int idx, ref string[] textStr)
        {

            if (textStr[idx].Contains(str1))  //�׾ȿ� �÷��� �����ϴ� �ܾ �ִٸ� <�� �ٿ���
                textStr[idx] = "<" + textStr[idx];

            if (textStr[idx].Contains(str2))  //�÷������ϴ� ������ �ܾ �ִٸ� 
            {
                int index = textStr[idx].IndexOf(">");  //color> �� ������ �򶧱��� ��ġ�� ���
                string strTemp = textStr[idx].Substring(index + 1);  //�� ��ġ �����ε������� �ִ� string�� ©��
                textStr[idx] = strTemp;   //©�� string�� ���� ������ �־���
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



                if (textStr.Length > 1)  //���� �÷��� b��ɾ �ִٸ�
                {
                  
                    if (strArrayOrder > textStr.Length)    //������ �� ������ �迭�Ǽ��� �Ѿ�� ������
                    {
                        EffectEnd();
                        yield break;
                    }

                    if (strArrayOrder < textStr.Length) //���ʴ�� ����
                    {
                        if (textStr[strArrayOrder].Equals("") && strArrayOrder == 0)      //�ɰ� �ؽ�Ʈ�� ���� ����ִٸ� �������� �Ѿ 
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

                            //if (textSpacing < maxSpacingText)   //���� ���� 
                            //{
                            //    if (textSpacing > minSpacingText)
                            //    {


                            //        if (textStr[strArrayOrder][idx].ToString() == " ")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += "\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else if (textStr[strArrayOrder][idx].ToString() == ",")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //            idx++;
                            //            nextLineCheck = true;

                            //        }
                            //        else  //�ƴϸ� �״�� ���
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
                            //        textEff += "\n" + bStartStr + textStr[strArrayOrder][idx] + stringB;  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //        idx++;
                            //        nextLineCheck = true;

                            //    }


                            //}

                            //msgText.text += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //idx++;
                        }



                        if (!colorCheck && !bCheck) //�÷��� ������
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
                            //        else  //�ƴϸ� �״�� ���
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
                            //        textEff += "\n" + textStr[strArrayOrder][idx];  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //        idx++;
                            //        nextLineCheck = true;
                            //    }



                            //}


                            //msgText.text += textStr[strArrayOrder][idx];
                            //idx++;
                        }
                        else if(colorCheck && !bCheck) //�÷��� �ִٸ�
                        {
                            if (textStr[strArrayOrder].Contains("<color="))  //���� �迭�� �÷��� Ī�ϴ� ���ڿ��� �ִٸ� �ش� ���ڿ��� ã�Ƽ� �и��� ���� �ϰ� ������ ���ڵ� �и��� ����
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


                            //if (textSpacing < maxSpacingText)   //���� ���� 
                            //{

                            //    if (textSpacing > minSpacingText)
                            //    {
                            //        if (textStr[strArrayOrder][idx].ToString() == " ")
                            //        {
                            //            textSpacing = 0;
                            //            textEff += "\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
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
                            //                    textEff += ",";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                    idx++;
                            //                    nextLineCheck = true;
                            //                }
                            //                else
                            //                {
                            //                    textSpacing = 0;
                            //                    textEff += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                    idx++;
                            //                    nextLineCheck = true;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                textSpacing = 0;
                            //                textEff += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                idx++;
                            //                nextLineCheck = true;
                            //            }

         

                            //        }
                            //        else  //�ƴϸ� �״�� ���
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
                            //        textEff += "\n" + startColorStr + textStr[strArrayOrder][idx] + colorStr;  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //        idx++;
                            //        textSpacing++;
                            //        nextLineCheck = true;

                            //    }


                            //}

                            //msgText.text += startColorStr + textStr[strArrayOrder][idx] + colorStr;
                            //idx++;
                        }

                        if (textStr[strArrayOrder].Length == idx)   //���ڼ��� �ε����� ���ٸ� �������� �Ѿ��
                        {
                            strArrayOrder++;                //�������� �Ѿ�����ؼ� �÷���
                            idx = 0;                    //�ε����� �ʱ�ȭ
  
       
                            if(strArrayOrder == textStr.Length - 1)    //������������ ��ĭ�̸�
                            {
                                if (textStr[strArrayOrder].Length == 0)
                                    if (textStr[strArrayOrder].Contains(""))
                                        strArrayOrder++;

                                
                            }

                            if (strArrayOrder >= textStr.Length)    //������ �� ������ �迭�Ǽ��� �Ѿ�� ������
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

                else    //�⺻���� ���̾�α׶�� �ִ� �״�� ���
                {
                    if (idx > textStr[strArrayOrder].Length)
                        break;


                    textEff += textStr[strArrayOrder][idx];
                    idx++;

                    //NextStringCheck(textStr, strArrayOrder, ref idx, ref nextLineCheck, ref textEff, ref textSpacing);


                    //if (textSpacing < maxSpacingText)   //���� ���� 
                    //{
                    //    if (idx > textStr[strArrayOrder].Length)
                    //        yield break;

                    //    if (textSpacing > minSpacingText)
                    //    {
                    //        if (textStr[strArrayOrder][idx].ToString() == " ")
                    //        {
                    //            textSpacing = 0;
                    //            textEff += "\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
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
                    //                    textEff += ",";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                    //                    idx++;
                    //                    nextLineCheck = true;
                    //                }
                    //                else
                    //                {
                    //                    textSpacing = 0;
                    //                    textEff += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                    //                    idx++;
                    //                    nextLineCheck = true;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                textSpacing = 0;
                    //                textEff += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                    //                idx++;
                    //                nextLineCheck = true;
                    //            }


                    //        }
                    //        else  //�ƴϸ� �״�� ���
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

                    if (textStr[strArrayOrder].Length == idx)   //���ڼ��� �ε����� ���ٸ� �������� �Ѿ��
                    {
                        idx = 0;                    //�ε����� �ʱ�ȭ
                        strArrayOrder++;

                        if (strArrayOrder >= textStr.Length)    //������ �� ������ �迭�Ǽ��� �Ѿ�� ������
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
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetMouseButtonDown(0))       //�����̽��� ������ ��ũ��Ʈ�� ���� ǥ��
                {

                    if (textEffectCo != null)
                        StopCoroutine(textEffectCo);


                    msgText.text = "";
                    textSkip = "";
                    textSkipSpacing = 0;
                    idx = 0;
                    strArrayOrder = 0;
                    colorSkipCheck = false;
                   
                    if (textSkipStr.Length <= 1)  //������� ��Ʈ���϶� (1��)
                    {

                        for (int ii = 0; ii < textSkipStr[strArrayOrder].Length; ii++)  //���� ��Ʈ���� ���̸�ŭ �ݺ�
                        {
                            if (idx >= textSkipStr[strArrayOrder].Length)
                                break;

                            textSkip += textSkipStr[strArrayOrder][idx];
                            idx++;

                            //NextStringCheck(textSkipStr, strArrayOrder,ref idx,ref nextLineSkipcheck,ref textSkip, ref textSkipSpacing);

                            //if (textSkipSpacing < maxSpacingText)
                            //{
                            //    if (textSkipSpacing > minSpacingText)  //34��° ���ڼ����� ������ ���Ⱑ �ִٸ�
                            //    {
                            //        if (textSkipStr[strArrayOrder][idx].ToString() == " ") //���Ⱑ �ִٸ� �Ѱܹ�����
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
                            //                    textSkip += ",";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                    idx++;
                            //                    nextLineSkipcheck = true;
                            //                }
                            //                else
                            //                {
                            //                    textSkipSpacing = 0;
                            //                    textSkip += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                    idx++;
                            //                    nextLineSkipcheck = true;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                textSpacing = 0;
                            //                textSkip += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //                idx++;
                            //                nextLineSkipcheck = true;
                            //            }

                            //        }
                            //        else  //�ƴϸ� �״�� ���
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
                            //else //textSkipSpacing�� maxġ�� ������
                            //{
                            //    if (textSkipStr[strArrayOrder][idx].ToString() == " ")
                            //    {
                            //        //���Ⱑ �ִٸ� ����ĭ���� �Ѿ�� ��������
                            //        textSkipSpacing = 0;
                            //        textSkip += "\n" + "";
                            //        idx++;
                            //        nextLineSkipcheck = true;

                            //    }
                            //    else //���Ⱑ ���ٸ� ����ĭ���� �Ѿ�鼭 �������� ���
                            //    {
                            //        textSkipSpacing = 0;
                            //        textSkip += "\n" + textSkipStr[strArrayOrder][idx];  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                            //        idx++;
                            //        nextLineSkipcheck = true;

                            //    }

                            //}
                        }
                    }

                    else    //�������ִ� ��Ʈ���϶� (���� �迭�� ������)
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
                                    if (textSkipStr[ii].Contains("<color="))  //���� �迭�� �÷��� Ī�ϴ� ���ڿ��� �ִٸ� �ش� ���ڿ��� ã�Ƽ� �и��� ���� �ϰ� ������ ���ڵ� �и��� ����
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
                                //        //�ε����� 0�϶� ���Ⱑ �ִٸ�
                                //        textSkip += "";     //��ĭ
                                //        idx++;
                                //        textSkipSpacing++;
                                //    }


                                //    if(textSkipSpacing > minSpacingText)  //40��° ���ڼ����� ������ ���Ⱑ �ִٸ�
                                //    {
                                //        if (textSkipStr[ii][idx].ToString() == " ") //���Ⱑ �ִٸ� �Ѱܹ�����
                                //        {
                                //            textSkipSpacing = 0;
                                //            textSkip += "\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
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
                                //                    textSkip += ",";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                                //                    idx++;
                                //                    nextLineSkipcheck = true;
                                //                }
                                //                else
                                //                {
                                //                    textSkipSpacing = 0;
                                //                    textSkip += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                                //                    idx++;
                                //                    nextLineSkipcheck = true;
                                //                }
                                //            }
                                //            else
                                //            {
                                //                textSpacing = 0;
                                //                textSkip += ",\n";  //�̰��ϴϱ� ó���� msg�� text���� �Ⱥ������������ ��ŵ�� �ȴ�(���ľ���)
                                //                idx++;
                                //                nextLineSkipcheck = true;
                                //            }


                                //        }
                                //        else  //�ƴϸ� �״�� ���
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
                                //        //���Ⱑ �ִٸ� ����ĭ���� �Ѿ�� ��������
                                //        textSkipSpacing = 0;
                                //        textSkip += "\n" + "";
                                //        idx++;
                                //        textSkipSpacing++;
                                //        nextLineSkipcheck = true;

                                //    }
                                //    else //���Ⱑ ���ٸ� ����ĭ���� �Ѿ�鼭 �������� ���
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

                                if (textSkipStr[ii].Length == idx)   //���ڼ��� �ε����� ���ٸ� �������� �Ѿ��
                                {
                                    idx = 0;                    //�ε����� �ʱ�ȭ

                                    if (ii + 1 >= textSkipStr.Length)    //������ �� ������ �迭�Ǽ��� �Ѿ�� ������
                                    {
                                        msgText.text = textSkip;
                                        EffectEnd();

                                        return;
                                    }


        


                                }
                            }
                        }
                         //�����ִ°Ŵ� �ٽ� ��Ʈ������ ���� �޾Ƽ� ��ü�� ǥ��


                    }
                    


                    msgText.text = textSkip;
                    EffectEnd();
                }

            }







        }


        void NextStringCheck(string[] str , int strArrayidx , ref int index, ref bool check, ref string textStr, ref int textSpacing)
        {
            //check�� Ʈ��� ������ �����ٷ� �̵��ߴٴ� �� 
            //���⼭ �����ٷ� �Ѿ������ \n�� üũ�ϰ� �����ش�.

            if (str[strArrayidx][str[strArrayidx].Length - 1].ToString() == "\n") //\n�� �����������϶�
            {
                textSpacing = 0;
                return;

            }


            if (check)  //Ȥ�� �ٲ������ �𸣱⶧���� true false �Ѵ� �Ȱ��� ����
            {
                if (str[strArrayidx][index].ToString() == "\n")  //true�϶�  check������ ���� ������ �� �ּ�ó���ϸ� ù°�ٿ��� ���Ⱑ ������ �����ٿ��� \n�� �۵�����
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
