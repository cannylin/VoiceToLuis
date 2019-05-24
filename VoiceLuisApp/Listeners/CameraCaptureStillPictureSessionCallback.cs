using Android.Hardware.Camera2;
using Android.Util;

namespace VoiceLuisApp.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly CameraBasic owner;

        public CameraCaptureStillPictureSessionCallback(CameraBasic owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // If something goes wrong with the save (or the handler isn't even 
            // registered, this code will toast a success message regardless...)
            owner.ShowToast("Saved: " + owner.mFile);
            Log.Debug(TAG, owner.mFile.ToString());
            owner.UnlockFocus();
        }
    }
}
