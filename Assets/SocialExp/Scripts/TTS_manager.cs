using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.Speech.Synthesis;
using Crosstales.RTVoice;
using TMPro;


public class TTS_manager : MonoBehaviour
{
    [SerializeField]
    private Button _buttonSpeak;
    [SerializeField]
    private TMP_InputField _text;

    [SerializeField]
    private Button _buttonRepeat;
    [SerializeField]
    private Button _buttonConfidence;
    [SerializeField]
    private Button[] _buttonConfidenceTasks;
    [SerializeField]
    private Button _buttonStopVoice;

    public static TTS_manager Instance { get; private set; }
    string voiceNameFR = "Microsoft Hortense Desktop";
    string voiceNameEN = "Microsoft David Desktop";
    AudioSource audioSource;
    Speaker speaker;
    [SerializeField]
    private AudioSource audioSourceRing;
    [SerializeField]
    private AudioSource audioSourceStart;

    bool isFrenchLanguage = true;
    // Use this for initialization
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
        speaker = gameObject.GetComponent<Speaker>();
        _buttonSpeak.onClick.AddListener(SpeakTTS);
        _buttonRepeat.onClick.AddListener(ReadInstructionTask);

        _buttonStopVoice.onClick.AddListener(StopVoice);

        _buttonConfidence.onClick.AddListener(ReadConfidence);
        for (int i = 0; i < _buttonConfidenceTasks.Length; i++)
        {
            _buttonConfidenceTasks[i].onClick.AddListener(ReadConfidence);
        }

    }

    public void StopVoice()
    {

        speaker.Silence();
    }
    public void SayText(string text)
    {
        if (isFrenchLanguage)
        {
            speaker.Speak(text, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            speaker.Speak(text, audioSource, speaker.VoiceForName(voiceNameEN));
        }

        Debug.Log("TTS: " + text);
    }
    public void SetLanguage(bool isFrench)
    {
        isFrenchLanguage = isFrench;
    }
    void SpeakTTS()
    {
        if (isFrenchLanguage)
        {
            speaker.Speak(_text.text, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            speaker.Speak(_text.text, audioSource, speaker.VoiceForName(voiceNameEN));
        }

        _text.text = "";
    }
    public void ReadInstructionTask()
    {

        int currentTask = SocialExpManager.Instance.GetCurrentTask() - 1;
        string toSay = "";
        if (isFrenchLanguage)
        {
            //French version
            switch (currentTask)
            {
                case 0:
                    //seatSearch
                    toSay = "Vous allez vous retrouver dans la salle d'attente d'un cabinet médical où vous devrez résoudre plusieurs tâches qui consistent en plusieurs essais. Les épreuves apparaîtront de manière aléatoire, de sorte que les épreuves précédentes ne vous aideront pas à trouver la solution de l'épreuve en cours. Votre première tâche consiste à trouver la seule chaise vide dans la salle d'attente parmi 6 chaises différentes. N'hésitez pas à vous déplacer, à vous rapprocher autant que vous le souhaitez des personnes et des objets pour accomplir la tâche. Une fois que vous pensez savoir où se trouve la chaise vide, vous devez marcher et vous arrêter juste devant, regarder en direction de la chaise vide, puis dire à haute voix que vous avez terminé. Vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 1:
                    //bodyOrientation
                    toSay = "Vous devez attendre que le médecin vous appelle et comme vous n'avez rien à faire, vous commencez à observer les autres personnes dans la salle d'attente. Vous verrez une personne entrer dans la pièce par la porte et s’arrêter devant vous. Votre tâche consiste à déterminer dans quelle direction la personne fait face : profil gauche ou droit, face à vous ou vous tournant le do. Une fois que vous identifiez la direction, vous devez l’énoncer à haute voix. Les directions apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 2:
                    //bodyAppearance
                    toSay = "Vous attendez toujours dans la salle d’attente. Maintenant, concentrez-vous sur l'apparence physique des personnes. Vous verrez une autre personne entrer dans la pièce par la porte et rester immobile devant vous. Votre tâche consiste à étudier 3 caractéristiques physiques de la personne : la tranche d'âge (enfant,adulte), le sexe (homme,femme) et un accessoire spécifique que la personne porte (stéthoscope,sac à main,bonnet ou écharpe). Vous pouvez vous rapprocher autant que vous le souhaitez de la personne afin de déterminer les informations demandées. Énoncer les caractéristiques à haute voix, vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 3:
                    //Body and face Emotion
                    toSay = "Vous êtes toujours dans la salle d'attente, occupé à observer les gens autour de vous. Une personne va entrer et se placer devant vous, avec une expression sur le visage et des gestes émotionnels. À l'aide de ses expressions faciales et des gestes de tout son corps, votre tâche consiste à déterminer quelle émotion cette personne exprime, parmi une émpotion positive, négative ou si la personne montre simplement une expression neutre. Les expressions apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Énoncez votre choix à haute voix, vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 4:
                    //faceEmotion
                    toSay = "Vous êtes toujours dans la salle d'attente, occupé à observer les gens autour de vous. Une personne va entrer et se placer devant vous, avec, cette fois, uniquement une expression sur son visage. Votre tâche est de déterminer quelle expression faciale cette personne exprime, parmi une émotion positive, négative ou si la personne montre simplement une expression neutre. Les expressions apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Énoncez votre choix à haute voix, vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 5:
                    //gestureRelationship
                    toSay = "Toujours dans la salle d'attente, vous observez une discussion entre deux personnes. Vous êtes curieux de connaître leur relation. Votre tâche est de choisir entre 3 types de relations : amicale (par exemple des amis, connaissance, ou de la famille), professionnel (par exemple entre médecin et patient) ou inconnus, se rencontrant pour la première fois dans cette salle d'attente. Les types de relation apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Enoncez votre choix à haute voix, vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 6:
                    //recognizeRole
                    toSay = "Finalement, vous voyez le médecin entrer dans la salle d'attente avec deux autres personnes. Ils s'arrêtent devant vous et engagent une conversation ensemble. Votre tâche consiste à déterminer qui est le médecin au sein du groupe. Le médecin porte une blouse médicale et un stéthoscope. N'hésitez pas à marcher vers les personnes pour les explorer si cela vous aide. Une fois que vous pensez savoir où se trouve le médecin, vous devez marcher vers lui, vous arrêter juste devant lui et dire que vous avez fait votre choix. Les positions apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 7:
                    //locomotion
                    toSay = "C'est enfin votre tour et vous devez suivre le médecin dans plusieurs couloirs jusqu'à la salle de consultation médicale. Si vous ne voyez pas le docteur, essayez de trouver votre chemin dans le couloir jusqu'à ce que vous le voyiez à nouveau. Vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 8:
                    //bodyLanguage
                    toSay = "Vous vous trouvez désormais au secrétariat, avec 3 personnes différentes autour de vous. Votre tâche est de savoir à qui s’adresse le médecin parmi les 3 personnes. Cela peut être la secrétaire, le patient ou vous. L'une des personnes peut être assise derrière un bureau. Les options apparaissent de manière aléatoire. Certaines peuvent ne jamais apparaître ou apparaître jusqu'à trois fois. Vous avez 2.5 minutes par essai, un signal sonore vous informera que vous êtes à la moitié du temps.";
                    break;
                case 9:
                    //observation
                    toSay = "Votre rendez-vous médical est terminé, vous êtes devant le cabinet médical pour rentrer chez vous. Au moment où vous sortez, vous voyez plusieurs personnes différentes. Votre tâche consiste à identifier le nombre de personnes et le plus grand nombre possible de caractéristiques des différentes personnes (telles que le sexe, l'âge et la position). Il n'y aura qu'un essai pour cette tâche de 4 minutes, un signal vous informera que vous êtes à la moitié du temps.";
                    break;

            }

            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {

            //English version
            switch (currentTask)
            {
                case 0:
                    //seatSearch
                    toSay = "You're going to find yourself in the waiting room of a medical practice where you will need to solve several tasks wich consist of several trials. The trials will appear in a randomly shuffled way so previous trials won't help you finding the solution of the current trial. Your first task is to find the only empty chair in the waiting room among 6 different chairs. Feel free to move around, getting as close to people and objects as you like to complete the task. Once you think you know where the empty chair is, walk and stop right in front of it, look in the direction of the empty chair, then say out loud that you've finished. You have 2.5 minutes per attempt, and an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 1:
                    //bodyOrientation
                    toSay = "You have to wait for the doctor to call you and as you have nothing to do, you start to observe the other people in the waiting room. You will see someone enter the room through the door and stop in front of you. Your task is to determine which direction the person is facing: left or right profile, facing you or with their back to you. Once you've identified the direction, you have to say it out loud. The directions appear randomly. Some may never appear or may appear up to three times. You have 2.5 minutes per attempt, and an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 2:
                    //bodyAppearance
                    toSay = "You're still waiting in the waiting room. Now focus on the physical appearance of the people. You will see another person enter the room through the door and stand still in front of you. Your task is to study 3 physical characteristics of the person: age range (child,adult), gender (male,female) and a specific accessory the person is wearing (stethoscope, handbag, beanie, or scarf). You can get as close to the person as you like to work out what information is required. Say the characteristics out loud, you have 2.5 minutes per attempt, an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 3:
                    //Body and faceEmotion
                    toSay = "You're still in the waiting room, busy observing the people around you. Someone will walk in and stand in front of you, with an expression on their face and emotional gestures. Using their facial expressions and whole body gestures, your task is to decide which emotion this person is expressing, whether it's a positive emotion, a negative emotion or simply a neutral expression. Announce your choice out loud. The expressions appear randomly. Some may never appear or may appear up to three times. You have 2.5 minutes per attempt, an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 4:
                    //faceEmotion
                    toSay = "You're still in the waiting room, busy observing the people around you. Someone will walk in and stand in front of you, with, this time, only an expression on their face. Your task is to decide which facial expression this person is expressing, whether it's a positive emotion, a negative emotion or simply a neutral expression. The expressions appear randomly. Some may never appear or may appear up to three times. Announce your choice out loud, you have 2.5 minutes per attempt, an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 5:
                    //gestureRelationship
                    toSay = "Still in the waiting room, you observe a discussion between two people. You are curious about their relationship. Your task is to choose between 3 types of relationship: friendly (for example friends, acquaintances, or family), professional (for example between doctor and patient) or strangers, meeting for the first time in this waiting room. Announce your choice out loud. The relationship types appear randomly. Some may never appear or may appear up to three times. You have 2.5 minutes per attempt, an acoustic signal will inform you that you are halfway through the time";
                    break;
                case 6:
                    //recognizeRole
                    toSay = "Finally, you see the doctor enter the waiting room with two other people. They stop in front of you and strike up a conversation together. Your task is to work out who the doctor is in the group. The doctor is wearing a medical coat and carrying a stethoscope. Feel free to walk towards the persons to explore them if it helps you. Once you think you know where the doctor is, you have to walk towards him, stop right in front of him and say that you have made your choice. The positions appear randomly. Some may never appear or may appear up to three times. You have 2.5 minutes for each attempt, and an acoustic signal will let you know when you're halfway through";
                    break;
                case 7:
                    //locomotion
                    toSay = "It's finally your turn and you have to follow the doctor through several corridors to the medical consultation room. If you don’t see doctor try to find your way through the corridor until you see him again. You have 2.5 minutes for each attempt, and an acoustic signal will tell you when you're halfway through the time";
                    break;
                case 8:
                    //bodyLanguage
                    toSay = "You are now in the secretary's office, with 3 different people around you. Your task is to find out which of the 3 people the doctor is talking to. It could be the secretary, the patient or you. One of the persons can be seated behind a desk. The options appear randomly. Some may never appear or may appear up to three times. You have 2.5 minutes for each attempt, and an acoustic signal will tell you when you're halfway through";
                break;
                case 9:
                    //observation
                    toSay = "Your doctor's appointment is over, you are outside the doctor's and on your way home. As you leave, you see several different people. Your task is to identify how many people there are and as many of the characteristics of the different people (such as gender, age and position) as possible. There will only be one attempt at this 4-minute task, and a signal will inform you that you are halfway through the time";
                    break;

            }

            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
    }
    public void ReadExperimentInstruction()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Vous venez juste d’arriver dans la salle d’attente chez le docteur et vous attendez votre tour. Vous êtes un humain entouré d’autres humains, vous pouvez bouger et observer. La plupart des tâches que vous allez réaliser sont passives, et elles vous seront expliquées au fur et à mesure, avec également ce qui est attendu de vous, et le temps que vous avez pour le réaliser.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "You've just arrived in the doctor's waiting room and are waiting your turn. You're a human surrounded by other humans, so you can move around and observe. Most of the tasks you're about to perform are passive, and they'll be explained to you as you go along, along with what's expected of you, and how much time you have to complete it";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }

    }
    public void ReadEndExperimentInstruction()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Merci d'avoir participé à cette expérience, vous pouvez maintenant retirer le casque de réalité virtuelle";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "Thank you for taking part in this experience. You can now remove your virtual reality headset.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }

    }
    public void ReadConfidence()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Dans quelle mesure êtes-vous sûr de votre choix ? (0 pas du tout, 5 très sûr)";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "How confident are you about your choice? (0 not at all, 5 very sure)";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
    }
    public void ReadConfidenceShort()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Dans quelle mesure êtes-vous sûr de votre choix ?";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "How confident are you about your choice?";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
    }
    public void RingHalfTime()
    {
        audioSourceRing.Play();
    }
    public void StartSound()
    {
        audioSourceStart.Play();
    }
    public void BodyAppearanceGender()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Vous devez déterminer le sexe de la personne se tenant en face de vous, entre homme ou femme.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "You must now determine the gender of the person standing in front of you, between male or female.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
    }
    public void BodyAppearanceAge()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Vous devez maintenant déterminer la tranche d’âge de la personne en face de vous, parmi enfant, ou adulte.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "You must now determine the age range of the person standing in front of you, among child or adult person.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
    }

    public void BodyAppearanceAccessory()
    {
        if (isFrenchLanguage)
        {
            string toSay = "Vous devez maintenant déterminer un accessoire spécifique que la personne porte. Tous les types d'objets peuvent se retrouver sur tous les types de personnes. L'accessoire peut être par exemple un stéthoscope, un sac à main, un bonnet, ou une écharpe.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameFR));
        }
        else
        {
            string toSay = "Now you need to determine a specific accessory the person is wearing. Every type of object can be on every type of person. Accessories could be a stethoscope, handbag, beanie, or scarf, for example.";
            speaker.Speak(toSay, audioSource, speaker.VoiceForName(voiceNameEN));
        }
   
    }

}
 
