using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;

public class TutorialNoteSpawner : MonoBehaviour
{
    [Tooltip("Heights of the note")]
    [SerializeField] private float noteHeight = 1.3f;
    [Tooltip("The first track")]
    [SerializeField] private Transform firstTrack;
    [Tooltip("The second track")]
    [SerializeField] private Transform secondTrack;
    [Tooltip("List of possible color for the note")]
    [SerializeField] private List<Material> noteMaterials;
    [Tooltip("The color of the note during auto perfect")]
    [SerializeField] private Material autoModeNoteMaterial;
    
    public Dictionary<int, Remover> trackRemoversMap = new Dictionary<int, Remover>();

    // Spawned note
    private GameObject spawnedNote;

    public static TutorialNoteSpawner instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            Remover tmpRemover;
            for (int i = 0; i < firstTrack.childCount; i++)
            {
                tmpRemover = firstTrack.GetChild(i).GetChild(0).GetComponent<Remover>();
                trackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            for (int i = 0; i < secondTrack.childCount; i++)
            {
                tmpRemover = secondTrack.GetChild(i).GetChild(0).GetComponent<Remover>();
                trackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            instance = this;
        }
    }

    public void spawnNote(int buttonId)
    {
        spawnedNote = ObjectPool.instance.getPooledObjTutorial();

        spawnedNote.transform.SetParent(trackRemoversMap[buttonId].transform);
        spawnedNote.transform.localRotation = Quaternion.identity;
        spawnedNote.transform.localPosition = Vector3.zero + transform.up * noteHeight;
        spawnedNote.SetActive(true);

        if (GameManager.instance.autoMode)
        {
            spawnedNote.GetComponent<Renderer>().material = autoModeNoteMaterial;
            spawnedNote.GetComponent<TutorialNote>().auto = true;
            spawnedNote.GetComponentInParent<ParticleSystemRenderer>().material = autoModeNoteMaterial;
        }
        else
            spawnedNote.GetComponent<Renderer>().material = noteMaterials[buttonId % 5];

        spawnedNote.GetComponent<TutorialNote>().startNote();
    }

}
