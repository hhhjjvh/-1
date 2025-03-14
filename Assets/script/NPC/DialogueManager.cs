using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [Header("UI Components")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText, nameTextl, nameTextr, descriptionText;
    public Image characterImagel, characterImager, backgroundImage;
    private enum State { Inactive, Scrolling, Choosing }
    private State currentState;

    private int currentLine = 0;

    private bool isScrolling = false;
    [SerializeField] private float scrollSpeed;
    private float defaultScrollSpeed;
    private NPCAbout npc;
    [Header("Dialogue Settings")]
    public DialogueSO currentDialogue;  // ��ǰ�Ի�������
    private string[] splitDialogueLines;  // �洢�ָ��ĶԻ�����
    private Dictionary<int, UnityEvent> eventMap;
    [Header("Branching Options")]
    public GameObject choicePanel;
    // public GameObject choiceButtonPrefab;
    private List<GameObject> currentChoices = new List<GameObject>();
    // ����UI���
    [Header("Title Settings")]
    public TextMeshProUGUI titleText;
    public float titleFontSize = 36f;
    public float titleDisplayTime = 2f;

    private float originalFontSize; // �洢ԭʼ�����С

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

        }
        dialogueBox.SetActive(false);
        choicePanel.SetActive(false);
        originalFontSize = dialogueText.fontSize;
        titleText.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        defaultScrollSpeed = scrollSpeed;
        //dialogueText.text = dialogueLines[currentLine];
        if (currentDialogue != null && !string.IsNullOrEmpty(currentDialogue.dialogueData))
        {
            splitDialogueLines = currentDialogue.cachedLines;
            dialogueText.text = splitDialogueLines[currentLine];
        }
        if (currentDialogue != null && currentDialogue.nodes[0].content != null)
        {
            splitDialogueLines = currentDialogue.nodes[0].content;
            dialogueText.text = splitDialogueLines[currentLine];
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != State.Scrolling) return;
        if (dialogueBox.activeInHierarchy && !choicePanel.activeInHierarchy && splitDialogueLines != null)
        {
            if (InputManager.Instance.canInteract || Input.GetKeyDown(KeyCode.Mouse0))
            {
                InputManager.Instance.canInteract = false;
                //Debug.Log("Interact with NPC");
                scrollSpeed = defaultScrollSpeed;
                if (!isScrolling)
                {

                    currentLine++;
                    if (currentLine >= splitDialogueLines.Length)
                    {
                        EndDialogue();
                    }
                    else
                    {

                        CheckName();
                        if (currentDialogue.systemType == DialogueSO.SystemType.NodeBased)
                        {
                            //Debug.Log("Node Based");
                            StartCoroutine(ProcessNodeSystem());
                        }
                        else
                        {
                            //  Debug.Log("Legacy System");
                            StartCoroutine(ProcessLegacySystem());
                        }

                    }
                }
                else
                {
                    string content = splitDialogueLines[currentLine];
                    var matches = Regex.Matches(content, @"(@[^ ]+)");
                    foreach (Match match in matches)
                    {
                        string fullTag = match.Groups[1].Value;
                        content = content.Replace(fullTag, "");
                    
                    }
                   
                    dialogueText.text = content;
                    descriptionText.text = dialogueText.text;
                    isScrolling = false;
                }

            }
        }

    }

    public void StartDialogue(DialogueSO dialogue, NPCAbout npc = null)
    {

        PlayerManager.instance.player.canMove = false;
        this.npc = npc;
        currentDialogue = dialogue;

        currentLine = 0;
        dialogueBox.SetActive(true);
        descriptionText.gameObject.SetActive(false);
        descriptionText.text = "";
        currentState = State.Scrolling;

        currentDialogue.ResetProgress();

        nameTextl.gameObject.SetActive(currentDialogue.hasName);
        nameTextr.gameObject.SetActive(currentDialogue.hasName);
        if (currentDialogue.systemType == DialogueSO.SystemType.NodeBased)
        {
            // Debug.Log("Node Based");
            splitDialogueLines = currentDialogue.nodes[currentDialogue.currentNodeIndex].content;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
        }
        else
        {
            splitDialogueLines = currentDialogue.cachedLines; // ֱ��ʹ��Ԥ��������
            CheckName();
            StartCoroutine(ProcessLegacySystem());
        }

    }
    #region Legacy System
    private IEnumerator ProcessLegacySystem()
    {
        eventMap = currentDialogue.events.ToDictionary(e => e.triggerLine, e => e.onReached);
        string triggerSymbol = "@trigger";
        if (eventMap != null && eventMap.TryGetValue(currentLine, out var evt))
        {
            splitDialogueLines[currentLine] = splitDialogueLines[currentLine].Replace(triggerSymbol, "");
            dialogueText.text = splitDialogueLines[currentLine];
            isScrolling = false;
            evt.Invoke();
        }

        yield return StartCoroutine(ScrollText(splitDialogueLines[currentLine]));
    }
    #endregion
    public void CheckName()
    {
        if (currentLine >= splitDialogueLines.Length) return;

        if (splitDialogueLines[currentLine].StartsWith("n-"))
        {
            backgroundImage.gameObject.SetActive(true);
            dialogueText.gameObject.SetActive(true);
            descriptionText.gameObject.SetActive(false);
            if (splitDialogueLines[currentLine].StartsWith("n-n"))
            {
                characterImagel.gameObject.SetActive(false);
                characterImager.gameObject.SetActive(false);
                backgroundImage.gameObject.SetActive(false);
                dialogueText.gameObject.SetActive(false);
                descriptionText.gameObject.SetActive(true);
            }
            else if (splitDialogueLines[currentLine].StartsWith("n-r"))
            {
                characterImager.gameObject.SetActive(true);
                characterImagel.gameObject.SetActive(false);
                nameTextr.text = splitDialogueLines[currentLine].Replace("n-r", "");
                Sprite icon = PoolMgr.Instance.GetSprite(nameTextr.text);
                {
                    characterImager.sprite = icon;
                }
            }
            else
            {
                characterImager.gameObject.SetActive(false);
                characterImagel.gameObject.SetActive(true);
                nameTextl.text = splitDialogueLines[currentLine].Replace("n-", "");
                Sprite icon = PoolMgr.Instance.GetSprite(nameTextl.text);
                if (icon != null)
                {
                    characterImagel.sprite = icon;
                }
            }

            if (currentLine + 1 >= splitDialogueLines.Length)
            {
                EndDialogue();
                return;
            }
            currentLine++;
            if (currentLine >= splitDialogueLines.Length)
            {
                EndDialogue();
                return;
            }

        }
    }
    private void EndDialogue()
    {
        PlayerManager.instance.player.canMove = true;

        if (currentDialogue.EndscriptableEffects != null)
        {
            foreach (var effect in currentDialogue.EndscriptableEffects)
            {
                if (effect != null)
                {
                    if (effect.ApplyTrigger())
                    {
                        // Debug.Log("Effect Applied");
                        effect.ApplyEffect(PlayerManager.instance.player.gameObject);

                    }

                }
            }
        }
        currentState = State.Inactive;
        dialogueBox.SetActive(false);
        choicePanel.SetActive(false);
        if (npc != null)
        {
            npc.isEntered = false;
        }
        currentLine = 0; // ���öԻ�����
        CGManager.Instance.HideCG();
    }

    #region Node Based System
    private IEnumerator ProcessNodeSystem()
    {
        var node = currentDialogue.nodes[currentDialogue.currentNodeIndex];

        // Debug.Log("Current Line: " + currentLine);
        if (node.triggerLine != -1 && node.triggerLine == currentLine)
        {
            //Debug.Log("Node Triggered"+ node.triggerLine);
            currentState = State.Choosing;
            ShowChoices(node.choices);
            splitDialogueLines[currentLine] = splitDialogueLines[currentLine].Replace(node.triggerSymbol, "");
            dialogueText.text = splitDialogueLines[currentLine];
            isScrolling = false;
            yield break;
        }

        yield return StartCoroutine(ScrollText(splitDialogueLines[currentLine]));

    }
    private void ShowChoices(Choice[] choices)
    {


        // ɾ���ɵ�ѡ��
        foreach (var choice in currentChoices)
        {

            PoolMgr.Instance.Release(choice);
            // Destroy(choice);
        }
        currentChoices.Clear();

        List<Choice> validChoices = new List<Choice>();
        foreach (var choice in choices)
        {
            if (MeetsChoiceRequirements(choice))
            {
                validChoices.Add(choice);
            }
        }
        if (validChoices.Count == 0)
        {
            splitDialogueLines = currentDialogue.nodes[0].content;
            currentDialogue.currentNodeIndex = 0;
            currentLine++;
            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
            // EndDialogue();
            return;
        }
        choicePanel.SetActive(true);
        // �����µ�ѡ�ť
        foreach (var choice in validChoices)
        {
            //// ���ѡ������
            //if (!MeetsChoiceRequirements(choice)) continue;

            GameObject button = PoolMgr.Instance.GetObj("ChoiceButton");
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
            button.transform.SetParent(choicePanel.transform, false);
            // button.gameObject.name= choice.text;
            currentChoices.Add(button);
        }
        // Ϊ��ť���õ���
        for (int i = 0; i < currentChoices.Count; i++)
        {
            var button = currentChoices[i].GetComponent<Button>();
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.Explicit;

            // �������µ���
            if (i > 0)
                nav.selectOnUp = currentChoices[i - 1].GetComponent<Button>();
            if (i < currentChoices.Count - 1)
                nav.selectOnDown = currentChoices[i + 1].GetComponent<Button>();

            button.navigation = nav;
        }

        // �Զ�ѡ�е�һ����ť
        if (currentChoices.Count > 0)
        {
            StartCoroutine(SetFirstSelected());
            //EventSystem.current.SetSelectedGameObject(currentChoices[0]);
        }
    }
    IEnumerator SetFirstSelected()
    {
        yield return null; // �ȴ�һ֡ȷ��UI��Ⱦ���
        if (currentChoices.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(currentChoices[0]);
        }
    }
    private bool MeetsChoiceRequirements(Choice choice)
    {
        // ʵ�������������߼������磺
        // if(Player.level < choice.minLevel) return false;
        // if(!string.IsNullOrEmpty(choice.requiredItem) && !Inventory.HasItem(choice.requiredItem)) return false;
        // �����������
        if (choice.requireTaskCompletion && choice.taskToTrigger != null)
        {
            foreach (var prerequisite in choice.taskToTrigger.prerequisites)
            {
                if (prerequisite.status != TaskSO.TaskStatus.Completed)
                    return false;
            }
        }
        if (choice.taskToTrigger != null && choice.taskToTrigger.status != TaskSO.TaskStatus.NotStarted)
        {
            return false;
        }
        return true;
    }

    private void OnChoiceSelected(Choice choice)
    {
        choicePanel.SetActive(false);
        choice.onSelected?.Invoke();
        bool canApply = true;
        foreach (var effect in choice.scriptableEffects)
        {
            if (effect != null)
            {
                if (effect.ApplyTrigger())
                {
                    Debug.Log("Effect Applied");
                    effect.ApplyEffect(PlayerManager.instance.player.gameObject);

                }
                else
                {
                    canApply = false;
                }


            }
        }
        if (choice.taskToTrigger != null)
        {
            bool taskAccepted = TaskManager.Instance.TryAcceptTask(choice.taskToTrigger);
            if (!taskAccepted)
            {
                canApply = false;
                // �������ȡʧ�ܣ����ڴ˴�������ʾ��Ч����ʾUI
                Debug.Log("�����ȡʧ�ܣ�����ǰ������");
            }
        }
        // Debug.Log("Choice Selected: " + canApply);
        if (!canApply && choice.EndingDialogueIndex != 0)
        {
            currentLine = 0;
            splitDialogueLines = currentDialogue.nodes[choice.EndingDialogueIndex].content;
            currentDialogue.currentNodeIndex = choice.EndingDialogueIndex;
            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
            return;
        }

        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue);
        }
        else
        {
            // Debug.Log(choice.nextDialogueIndex  +"   " + currentDialogue.nodes.Length);

            if (choice.nextDialogueIndex < currentDialogue.nodes.Length && choice.nextDialogueIndex > 0)
            {
                currentLine = 0;
                splitDialogueLines = currentDialogue.nodes[choice.nextDialogueIndex].content;
                currentDialogue.currentNodeIndex = choice.nextDialogueIndex;
            }
            else
            {
                splitDialogueLines = currentDialogue.nodes[0].content;
                currentDialogue.currentNodeIndex = 0;
                currentLine++;
                // Debug.Log("Current Line: " + currentLine);
            }

            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
        }

    }
    #endregion
    private IEnumerator ScrollText(string content)
    {
        dialogueText.text = "";
        isScrolling = true;

        // ��ȡ��������������ָ��
        var commands = new List<IEnumerator>();
        content = ProcessSpecialCommands(content, ref commands);

        // ��˳��ִ������ָ��
        foreach (var cmd in commands)
        {
            yield return StartCoroutine(cmd);
        }

        while (dialogueText.text.Length < content.Length && isScrolling)
        {
            dialogueText.text += content[dialogueText.text.Length];
            descriptionText.text = dialogueText.text;
            yield return new WaitForSeconds(scrollSpeed);
        }
        isScrolling = false;
    }
    private string ProcessSpecialCommands(string content, ref List<IEnumerator> commands)
    {
        // ʹ������ƥ������ָ��
        var matches = Regex.Matches(content, @"(@[^ ]+)");
        foreach (Match match in matches)
        {
            string fullTag = match.Groups[1].Value;

            // ����CGָ��
            if (fullTag.StartsWith("@cg="))
            {
                var cmd = HandleCGCommand(fullTag);
                if (cmd != null) commands.Add(cmd);
                content = content.Replace(fullTag, "");
            }
            // ����ȴ�ָ��
            else if (fullTag.StartsWith("@wait="))
            {
                commands.Add(HandleWaitCommand(fullTag));
                content = content.Replace(fullTag, "");
            }
            // ��������ָ��
            else if (fullTag.StartsWith("@music="))
            {
                commands.Add(HandleMusicCommand(fullTag));
                content = content.Replace(fullTag, "");
            }
            else if (fullTag.StartsWith("@title="))
            {
                commands.Add(HandleTitleCommand(fullTag));
                content = content.Replace(fullTag, "");
            }
        }

        return content.Trim();
    }

    #region �첽ָ���
    private IEnumerator HandleCGCommand(string tag)
    {
        var match = Regex.Match(tag, @"@cg=([^,]+),?([\d.]*)");
        if (!match.Success) yield break;

        string cgName = match.Groups[1].Value.Trim();
        float fadeTime = 0.1f;
        if (!string.IsNullOrEmpty(match.Groups[2].Value))
            float.TryParse(match.Groups[2].Value, out fadeTime);

        bool isDone = false;
        CGManager.Instance.ShowCGWithFade(cgName, fadeTime, () => isDone = true);
        yield return new WaitUntil(() => isDone);
    }

    private IEnumerator HandleWaitCommand(string tag)
    {
        var match = Regex.Match(tag, @"@wait=([\d.]+)");
        if (!match.Success) yield break;

        if (float.TryParse(match.Groups[1].Value, out float waitTime))
        {
            yield return new WaitForSeconds(waitTime); // ��ȷʹ��Э�̵ȴ�
        }
    }

    private IEnumerator HandleMusicCommand(string tag)
    {
        var match = Regex.Match(tag, @"@music=([\w]+)");
        if (!match.Success) yield break;

        string musicName = match.Groups[1].Value;
        if (!string.IsNullOrEmpty(musicName))
        {
            AudioMgr.Instance.PlaySFX(musicName); // ������������
        }
        yield return null; // ����ȴ����ֲ������
    }
    private IEnumerator HandleTitleCommand(string tag)
    {
        var match = Regex.Match(tag, @"@title=([^,]+),?([\d.]*)");
        if (!match.Success) yield break;

        string titleContent = match.Groups[1].Value;
        float displayTime = titleDisplayTime;

        // ������ѡ��ʱ�����
        if (!string.IsNullOrEmpty(match.Groups[2].Value))
            float.TryParse(match.Groups[2].Value, out displayTime);

        // ��ʾ����
        titleText.text = titleContent;
        titleText.fontSize = titleFontSize;
        titleText.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(false);
       descriptionText.gameObject.SetActive(false);

        // ��ͣ�Ի�����
        bool originalScrollingState = isScrolling;
        isScrolling = false;
        StartCoroutine(FadeTitle(0f, 1f, 1f)); // ����
        yield return new WaitForSeconds(displayTime);
        StartCoroutine(FadeTitle(1f, 0f, 1f)); // ����
       
        // �ָ��Ի�����
        isScrolling = originalScrollingState;
        titleText.gameObject.SetActive(false);
       
    }
    private IEnumerator FadeTitle(float from, float to, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            titleText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}
