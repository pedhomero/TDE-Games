using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("⚙️ Configurações do Turno")]
    public int currentPlayer = 1;
    public float turnDuration = 20f; // ⏱ Tempo do turno em segundos
    private float turnTimer;

    [Header("🚶 Limite de Movimento")]
    public int maxSteps = 6; // quantidade máxima de passos por turno
    private int currentSteps = 0;

    [Header("🎯 UI do Turno")]
    public Text turnIndicatorText;
    public Text timerText;
    public Slider stepSlider;

    private NewMonoBehaviourScript[] playersInScene;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playersInScene = FindObjectsOfType<NewMonoBehaviourScript>();
        StartTurn(1); // começa pelo Player 1
    }

    void Update()
    {
        // Se não tiver iniciado o turno, não faz nada
        if (turnTimer <= 0f)
            return;

        // ⏳ Contagem regressiva
        turnTimer -= Time.deltaTime;
        if (turnTimer <= 0f)
        {
            Debug.Log("⏰ Tempo acabou! Passando a vez...");
            NextTurn();
        }

        UpdateUI();
    }

    // 🔁 Inicia o turno de um jogador específico
    public void StartTurn(int player)
    {
        currentPlayer = player;
        currentSteps = 0; // reseta passos
        turnTimer = turnDuration; // reinicia tempo

        Debug.Log("🎮 Iniciando turno do Player " + currentPlayer);

        UpdateUI();
    }

    // ⏭ Passa para o próximo jogador
    public void NextTurn()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        StartTurn(currentPlayer);
    }

    // ✅ Verifica se é o turno do jogador
    public bool IsMyTurn(int playerNumber)
    {
        return playerNumber == currentPlayer;
    }

    // 🚶‍♂️ Usa um passo — retorna true se ainda puder andar
    public bool TryUseStep()
    {
        if (currentSteps < maxSteps)
        {
            currentSteps++;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("🚫 Player " + currentPlayer + " usou todos os passos!");
            return false;
        }
    }

    // 🖥️ Atualiza interface visual
    void UpdateUI()
    {
        if (turnIndicatorText != null)
            turnIndicatorText.text = "🎮 Player " + currentPlayer + " - Sua Vez";

        if (timerText != null)
            timerText.text = "⏱ " + Mathf.Ceil(turnTimer).ToString() + "s";

        if (stepSlider != null)
        {
            stepSlider.maxValue = maxSteps;
            stepSlider.value = currentSteps;
        }
    }
}
