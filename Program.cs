using Stride.CommunityToolkit.Bepu;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Rendering.Compositing;
using Stride.Engine;
using Stride.Games;
using GumStride;

using var game = new Game();

game.Run(start: Start);

void Start(Scene rootScene)
{
	game.AddGraphicsCompositor()
	.AddCleanUIStage()
	.AddSceneRenderer(new SkiaGumSceneRenderer());

	game.Add3DCamera().Add3DCameraController();
	game.AddDirectionalLight();
	game.Add3DGround();
}
