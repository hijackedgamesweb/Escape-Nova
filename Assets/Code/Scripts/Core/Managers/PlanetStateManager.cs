using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;
using Code.Scripts.Core.World.ConstructableEntities;

public class PlanetStateManager : MonoBehaviour
{
    
    public enum Estado
    {
        Construyendo,
        Produciendo,
        Colonizado,
        EnConflicto,
        Conquistado,
        Destruido
    }

    [Header("Estado actual")]
    public Estado estadoActual;

    private Animator animator;
    private Planet _planet;

    void Awake()
    {
        _planet = GetComponentInParent<Planet>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (_planet != null)
        {
            _planet.OnConstructionCompleted += FinConstruido;
        }
    }

    void OnDisable()
    {
        if (_planet != null)
        {
            _planet.OnConstructionCompleted -= FinConstruido;
        }
    }

    void Start()
    {
        CambiarEstado(Estado.Construyendo);
    }
    void Update()
    {
        
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;

        if (animator != null)
        {
            animator.SetInteger("Estado", (int)estadoActual);
        }

        Debug.Log("Estado cambiado a: " + estadoActual);
    }

    

    
    public void FinConstruido()
    {
        if (estadoActual == Estado.Construyendo)
            CambiarEstado(Estado.Produciendo);
    }


    public void EstablecerContacto()
    {
        if (estadoActual == Estado.Produciendo)
            CambiarEstado(Estado.Colonizado);
    }

    
    public void DeclararGuerra()
    {
        if (estadoActual == Estado.Colonizado || estadoActual == Estado.Conquistado )
            CambiarEstado(Estado.EnConflicto);
    }

    
    public void VencerGuerra()
    {
        if (estadoActual == Estado.EnConflicto)
            CambiarEstado(Estado.Colonizado);
    }

    
    public void PerderGuerra()
    {
        if (estadoActual == Estado.EnConflicto)
            CambiarEstado(Estado.Conquistado);
    }

    
    public void Destruir()
    {
        if (estadoActual == Estado.Colonizado || estadoActual == Estado.Produciendo)
            CambiarEstado(Estado.Destruido);
    }
}




