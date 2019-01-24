using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SS14.Shared.Utility;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace SS14.Client.Graphics
{
    internal partial class DisplayManagerOpenGL
    {
        private readonly Dictionary<int, LoadedTexture> _loadedTextures = new Dictionary<int, LoadedTexture>();
        private int _nextTextureId;

        public Texture LoadTextureFromPNGStream(Stream stream)
        {
            DebugTools.Assert(_mainThread == Thread.CurrentThread);

            using (var image = (System.Drawing.Bitmap) System.Drawing.Image.FromStream(stream))
            {
                GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
                GL.TextureStorage2D(texture, 1, SizedInternalFormat.Rgba8, image.Width, image.Height);
                GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
                GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

                var data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TextureSubImage2D(texture, 0, 0, 0, image.Width, image.Height, PixelFormat.Bgra,
                    PixelType.UnsignedByte, data.Scan0);

                image.UnlockBits(data);
                var loaded = new LoadedTexture
                {
                    OpenGLObject = texture,
                    Width = image.Width,
                    Height = image.Height
                };

                var id = ++_nextTextureId;
                _loadedTextures.Add(id, loaded);

                return new OpenGLTexture(id, image.Width, image.Height);
            }
        }

        public Texture LoadTextureFromImage<T>(Image<T> image) where T : struct, IPixel<T>
        {
            DebugTools.Assert(_mainThread == Thread.CurrentThread);

            if (typeof(T) != typeof(Rgba32))
            {
                throw new NotImplementedException("Cannot load images other than Rgba32");
            }

            GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
            GL.TextureStorage2D(texture, 1, SizedInternalFormat.Rgba8, image.Width, image.Height);
            GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

            var span = ((Image<Rgba32>) (object) image).GetPixelSpan();
            unsafe
            {
                fixed (Rgba32* ptr = &MemoryMarshal.GetReference(span))
                {
                    GL.TextureSubImage2D(texture, 0, 0, 0, image.Width, image.Height, PixelFormat.Rgba,
                        PixelType.UnsignedByte, (IntPtr) ptr);
                }
            }

            var loaded = new LoadedTexture
            {
                OpenGLObject = texture,
                Width = image.Width,
                Height = image.Height
            };

            var id = ++_nextTextureId;
            _loadedTextures.Add(id, loaded);

            return new OpenGLTexture(id, image.Width, image.Height);
        }

        private class LoadedTexture
        {
            public int OpenGLObject;
            public int Width;
            public int Height;
        }
    }
}
