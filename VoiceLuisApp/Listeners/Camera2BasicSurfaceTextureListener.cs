using Android.Views;

namespace VoiceLuisApp.Listeners
{
    public class VoiceLuisAppSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly CameraBasic owner;

        public VoiceLuisAppSurfaceTextureListener(CameraBasic owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
        {

        }
    }
}