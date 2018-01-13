using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(PostProcessingBehaviour))]

public class PPManager : MonoBehaviour {
	[SerializeField]
	private PostProcessingProfile m_Profile;
	[SerializeField]
	private bool heavyEffect = true;

	void OnEnable()
	{
		var behaviour = GetComponent<PostProcessingBehaviour>();

		if (behaviour.profile == null)
		{
			enabled = false;
			return;
		}

		m_Profile = Instantiate(behaviour.profile);
		behaviour.profile = m_Profile;
		m_Profile.motionBlur.enabled = true;
	}

	void Update()
	{
//		var vignette = m_Profile.vignette.settings;
//		vignette.smoothness = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) * 0.99f) + 0.01f;
//		m_Profile.vignette.settings = vignette;
	}

	public bool SetAlcoholValue(float value){
		value = Mathf.Clamp01(Mathf.Abs (value));

		var motion = m_Profile.motionBlur.settings;
		motion.shutterAngle = value;
		motion.frameBlending = value;
		m_Profile.motionBlur.settings = motion;

		if (m_Profile.chromaticAberration.enabled == false)
			m_Profile.chromaticAberration.enabled = true;
		var aberration = m_Profile.chromaticAberration.settings;
		aberration.intensity = value;
		m_Profile.chromaticAberration.settings = aberration;

		if (m_Profile.vignette.enabled == false)
			m_Profile.vignette.enabled = true;
		var vignette = m_Profile.vignette.settings;
		vignette.smoothness = Mathf.Clamp01 (value + 0.01f);
		m_Profile.vignette.settings = vignette;

		return true;
	}

	public bool toggleHeavyEffect(){
		m_Profile.ambientOcclusion.enabled = !heavyEffect;
		m_Profile.antialiasing.enabled = !heavyEffect;
		m_Profile.bloom.enabled = !heavyEffect;
		heavyEffect = !heavyEffect;
		return heavyEffect;
	}
}
