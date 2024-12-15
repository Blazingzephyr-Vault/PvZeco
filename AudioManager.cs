using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	private AudioSource audioSource;

	public AudioSource wetherAudio;

	private List<EFAudio> CurrEFAudios = new List<EFAudio>();

	private Dictionary<string, int> EfAudioNum = new Dictionary<string, int>();

	public float BgmVolume = 1f;

	public float SoundVolume = 1f;

	private Coroutine WeatherCoroutine;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		audioSource = GetComponent<AudioSource>();
	}

	public void SetVolume(float sound, float bgm)
	{
		wetherAudio.volume = sound;
		audioSource.volume = bgm;
		BgmVolume = bgm;
		SoundVolume = sound;
	}

	public void SetWeatherVolume(bool isStart, bool needFade)
	{
		if (WeatherCoroutine != null)
		{
			StopCoroutine(WeatherCoroutine);
		}
		if (needFade)
		{
			WeatherCoroutine = StartCoroutine(ChangeWeather(isStart));
		}
		else if (isStart)
		{
			wetherAudio.volume = SoundVolume;
		}
		else
		{
			wetherAudio.volume = 0f;
		}
		wetherAudio.enabled = isStart;
	}

	private IEnumerator ChangeWeather(bool isStart)
	{
		if (isStart)
		{
			wetherAudio.enabled = true;
			wetherAudio.volume = 0f;
			while (wetherAudio.volume < SoundVolume)
			{
				yield return new WaitForSeconds(0.2f);
				wetherAudio.volume += 0.01f;
			}
		}
		else
		{
			while (wetherAudio.volume > 0f)
			{
				yield return new WaitForSeconds(0.2f);
				wetherAudio.volume -= 0.01f;
			}
			wetherAudio.enabled = false;
		}
	}

	public EFAudio PlayEFAudio(AudioClip clip, Vector2 Pos, bool isAll = false)
	{
		bool flag = false;
		if (EfAudioNum.ContainsKey(clip.name))
		{
			if (EfAudioNum[clip.name] < 5)
			{
				flag = true;
				EfAudioNum[clip.name]++;
			}
		}
		else
		{
			flag = true;
			EfAudioNum.Add(clip.name, 1);
		}
		if (!flag)
		{
			return null;
		}
		EFAudio eFAudio = null;
		if (isAll)
		{
			eFAudio = PoolManager.Instance.GetObj(GameManager.Instance.AudioConf.EFAudio).GetComponent<EFAudio>();
			eFAudio.Init(clip, SoundVolume);
			eFAudio.transform.SetParent(base.transform);
		}
		else if (MapManager.Instance.GetCurrMap(Pos) == CameraControl.Instance.CurrMap)
		{
			eFAudio = PoolManager.Instance.GetObj(GameManager.Instance.AudioConf.EFAudio).GetComponent<EFAudio>();
			eFAudio.Init(clip, SoundVolume);
			eFAudio.transform.SetParent(base.transform);
			CurrEFAudios.Add(eFAudio);
		}
		else if (EfAudioNum.ContainsKey(clip.name))
		{
			EfAudioNum[clip.name]--;
			if (EfAudioNum[clip.name] <= 0)
			{
				EfAudioNum.Remove(clip.name);
			}
		}
		return eFAudio;
	}

	public void RemoveEFAudio(EFAudio eFAudio, string name)
	{
		if (CurrEFAudios.Contains(eFAudio))
		{
			CurrEFAudios.Remove(eFAudio);
		}
		if (EfAudioNum.ContainsKey(name))
		{
			EfAudioNum[name]--;
			if (EfAudioNum[name] <= 0)
			{
				EfAudioNum.Remove(name);
			}
		}
	}

	public void ChangeMapReset()
	{
		for (int i = 0; i < CurrEFAudios.Count; i++)
		{
			CurrEFAudios[i].Close();
		}
	}

	public void PlayBgAudio(AudioClip clip)
	{
		if (clip == null)
		{
			audioSource.UnPause();
			return;
		}
		audioSource.Stop();
		audioSource.clip = clip;
		audioSource.Play();
	}

	public void FadeBgAndPlayNew(AudioClip clip)
	{
		StartCoroutine(FadeBg(clip));
	}

	private IEnumerator FadeBg(AudioClip clip)
	{
		while (audioSource.volume > 0f)
		{
			yield return new WaitForSeconds(0.2f);
			audioSource.volume -= 0.01f;
		}
		audioSource.volume = BgmVolume;
		audioSource.Stop();
		audioSource.clip = clip;
		audioSource.Play();
	}

	public void StopBgAudio()
	{
		audioSource.Pause();
	}
}
