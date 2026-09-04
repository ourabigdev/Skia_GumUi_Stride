using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SkiaSharp;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Compositing;

namespace GumStride;

public class SkiaGumSceneRenderer : SceneRendererBase
{
	private SKSurface? _skSurface;
	private SKCanvas? _skCanvas;
	private Texture? _skiaTexture;
	private SpriteBatch? _spriteBatch;

	private int _width = 1280;
	private int _height = 720;

	public SKCanvas? Canvas => _skCanvas;

	protected override void InitializeCore()
	{
		base.InitializeCore();

		_spriteBatch = new SpriteBatch(GraphicsDevice);
		RecreateSurface(GraphicsDevice.Presenter.BackBuffer.Width, GraphicsDevice.Presenter.BackBuffer.Height);
	}

	private void RecreateSurface(int width, int height)
	{
		if(width <= 0 || height <= 0) return;

		_width = width;
		_height = height;

		_skSurface?.Dispose();
		_skiaTexture?.Dispose();

		var info = new SKImageInfo(_width, _height, SKColorType.Rgba8888, SKAlphaType.Premul);
		_skSurface = SKSurface.Create(info);
		_skCanvas = _skSurface.Canvas;

		_skiaTexture = Texture.New2D(
			GraphicsDevice,
			_width,
			_height,
			PixelFormat.R8G8B8A8_UNorm,
			TextureFlags.ShaderResource,
			1,
			GraphicsResourceUsage.Dynamic);
	}

	protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
	{
		var commandList = drawContext.CommandList;
		var backBuffer = drawContext.CommandList.RenderTarget;
		if(backBuffer.Width != _width || backBuffer.Height != _height)
		{
			RecreateSurface(backBuffer.Width, backBuffer.Height);
		}

		if(_skCanvas == null || _skSurface == null || _skiaTexture == null || _spriteBatch == null)
			return;

		_skCanvas.Clear(SKColors.Empty);

		RenderSkiaUI(_skCanvas);
		_skCanvas.Flush();

		SKPixmap pixmap = _skSurface.PeekPixels();
		IntPtr pixelPointer = pixmap.GetPixels();
		int byteSize = _width * _height * 4;

		_skiaTexture.SetData(commandList, new DataPointer(pixelPointer, byteSize));
		commandList.SetRenderTarget(drawContext.CommandList.DepthStencilBuffer, backBuffer);

		_spriteBatch.Begin(drawContext.GraphicsContext);
		_spriteBatch.Draw(_skiaTexture, new RectangleF(0, 0, _width, _height), color: Color.White);
		_spriteBatch.End();
	}

	

	private void RenderSkiaUI(SKCanvas canvas)
	{
		using var paint = new SKPaint { Color = SKColors.OrangeRed, IsAntialias = true };
		canvas.DrawRoundRect(new SKRect(50, 50, 400, 200), 16, 16, paint);

		using var font = new SKFont(SKTypeface.FromFamilyName("Arial"), 32);
		using var textPaint = new SKPaint {Color = SKColors.White, IsAntialias = true };
		canvas.DrawText(SKTextBlob.Create("Stride Skia Rendering", font), 80, 130, textPaint);
	}

	protected override void Destroy()
	{
		_skSurface?.Dispose();
		_skiaTexture?.Dispose();
		_spriteBatch?.Dispose();
		base.Destroy();
	}
}