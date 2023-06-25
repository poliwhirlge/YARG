using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using YARG.Settings;

namespace YARG.PlayMode {
	public class CameraPositioner : MonoBehaviour {
		private static readonly List<CameraPositioner> CameraPositioners = new();

		private bool _entryAnimationQueued;

		private void Start() {
			CameraPositioners.Add(this);

			UpdateAntiAliasing();

			if (_entryAnimationQueued) {
				StartCoroutine(PlayEntryAnimation());
			} else {
				UpdatePosition();
			}
		}

		private void OnDestroy() {
			CameraPositioners.Remove(this);
		}

		private void UpdateAntiAliasing() {
			// Set anti-aliasing
			var info = GetComponent<UniversalAdditionalCameraData>();
			if (SettingsManager.Settings.LowQuality.Data) {
				info.antialiasing = AntialiasingMode.None;
			} else {
				info.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
				info.antialiasingQuality = AntialiasingQuality.Low;
			}
		}

		private void UpdatePosition() {
			// FOV
			GetComponent<Camera>().fieldOfView = SettingsManager.Settings.TrackCamFOV.Data;

			// Position
			float y = SettingsManager.Settings.TrackCamYPos.Data;
			float z = SettingsManager.Settings.TrackCamZPos.Data - 6f;
			transform.localPosition = new Vector3(0f, y, z);

			// Rotation
			transform.localRotation = Quaternion.Euler(SettingsManager.Settings.TrackCamRot.Data, 0f, 0f);
		}

		public void QueueEntryAnimation() {
			_entryAnimationQueued = true;
		}

		private IEnumerator PlayEntryAnimation() {
			// Calculate the target position
			float targetY = SettingsManager.Settings.TrackCamYPos.Data;
			float targetZ = SettingsManager.Settings.TrackCamZPos.Data - 6f;
			var targetPosition = new Vector3(0f, targetY, targetZ);

			// Calculate the starting position (above the target)
			var startingPosition = new Vector3(0f, targetY + 3f, targetZ + 5f); // Adjust the translation distance here

			// Store the final position before hiding the camera
			var finalPosition = targetPosition;

			// Set the camera position to the starting position
			transform.localPosition = startingPosition;

			// Update the position and store the correct final position
			UpdatePosition();
			finalPosition = transform.localPosition;

			// Snap the camera to an initial position
			transform.localPosition = finalPosition + new Vector3(0f, 3f, 5f);

			// Entry animation duration (2 seconds)
			float animationDuration = 2.5f;
			float animationStartTime = Time.time;

			while (Time.time - animationStartTime < animationDuration) {
				float t = (Time.time - animationStartTime) / animationDuration; // Interpolation factor (0 to 1)

				// Apply a slower animation curve during move-in
				float smoothT = Mathf.SmoothStep(0f, 1f, t);
				smoothT = Mathf.Lerp(0f, smoothT, smoothT); // Apply additional smoothing


				// Smoothly interpolate the camera position using the modified curve
				transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, smoothT);

				yield return null;
			}

			// Ensure the final position is accurate
			transform.localPosition = finalPosition;
		}

		public static void UpdateAllAntiAliasing() {
			foreach (var cameraPositioner in CameraPositioners) {
				cameraPositioner.UpdateAntiAliasing();
			}
		}

		public static void UpdateAllPosition() {
			foreach (var cameraPositioner in CameraPositioners) {
				cameraPositioner.UpdatePosition();
			}
		}
	}
}
