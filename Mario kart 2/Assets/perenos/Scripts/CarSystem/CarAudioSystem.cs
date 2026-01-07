using UnityEngine;
using UnityEngine.Audio;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Audio System")]
    public class CarAudioSystem : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private AudioClip engineClip;
        [SerializeField] private AudioClip skidClip;
        [SerializeField] private AudioResource hitClip;
        [Range(0, 1)] public float skidVolume = 1;

        [Header("Pitch")]
        [Range(0.5f, 1)] public float pitchMultiplier = 1f;
        [Range(.5f, 3)] public float lowPitchMin = 1f;
        [Range(2, 7)] public float lowPitchMax = 6f;
        [Range(0, 1)] public float highPitchMultiplier = 0.25f;

        private CarController controller;

        private Transform tr;
        private float accFade = 0;
        private float acceleration;
        private float maxRolloffDistance = 500;
        private AudioSource engine;
        private AudioSource skid;
        private AudioSource hit;
        private bool m_StartedSound;
        private GameObject audioObject;
        private Camera mainCam;
        private float camDist;

        void Awake()
        {
            controller = GetComponent<CarController>();
            tr = transform;
            mainCam = Camera.main;
        }

        private void StartSound()
        {
            audioObject = new GameObject();
            audioObject.transform.SetParent(tr, false);
            audioObject.transform.name = "audio";
            if (engineClip != null) engine = SetUpEngineAudioSource(engineClip);
            if (skidClip != null) skid = SetupSkidSound(skidClip);
            if (hitClip != null) hit = SetupHitSound(hitClip);

            m_StartedSound = true;
        }

        private void StopSound()
        {
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }
            m_StartedSound = false;
        }

        void Update()
        {
            camDist = (mainCam.transform.position - tr.position).sqrMagnitude;

            accFade = Mathf.Lerp(accFade, Mathf.Abs(acceleration), 20 * Time.deltaTime);
            if (m_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
            {
                StopSound();
            }
            if (!m_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
            {
                StartSound();
            }
            if (m_StartedSound)
            {
                float pitch = CarController.ULerp(lowPitchMin, lowPitchMax, controller.Revs);
                pitch = Mathf.Min(lowPitchMax, pitch);

                engine.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                engine.volume = 1;

                skid.volume = controller.IsDrifting ? skidVolume : 0;
            }
        }

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.clip = clip;
            if (mixerGroup) source.outputAudioMixerGroup = mixerGroup;
            source.volume = 0;
            source.spatialBlend = 1;
            source.loop = true;
            source.dopplerLevel = 1;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.reverbZoneMix = 0;
            source.maxDistance = maxRolloffDistance;
            return source;
        }

        private AudioSource SetupSkidSound(AudioClip clip)
        {
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.clip = clip;
            if (mixerGroup) source.outputAudioMixerGroup = mixerGroup;
            source.volume = 0;
            source.spatialBlend = 1;
            source.loop = true;
            source.dopplerLevel = 1;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.playOnAwake = false;
            return source;
        }

        private AudioSource SetupHitSound(AudioResource resource)
        {
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.resource = resource;
            if (mixerGroup) source.outputAudioMixerGroup = mixerGroup;
            source.volume = 1;
            source.spatialBlend = 1;
            source.loop = false;
            source.dopplerLevel = 1;
            // source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.playOnAwake = false;
            return source;
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.contacts.Length > 0)
            {
                if (hit == null) return;
                hit.Play();
            }
        }
    }
}
