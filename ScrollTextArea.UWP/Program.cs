using StereoKit;
using System;
using Topten.RichTextKit;

namespace ScrollTextArea.UWP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize StereoKit
            SKSettings settings = new SKSettings
            {
                appName = "ScrollTextArea.UWP",
                assetsFolder = "Assets",
            };
            if (!SK.Initialize(settings))
                Environment.Exit(1);


            // Create assets used by the app
            Pose cubePose = new Pose(0, 0, -0.5f, Quat.Identity);
            Model cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);

            Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            Material floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;



            var rs = new RichString()
                .Alignment(TextAlignment.Center)
                .FontFamily("Segoe UI")
                .MarginBottom(20)
                .Add("Heading", fontSize: 24, fontWeight: 700, fontItalic: true)
                .Paragraph().Alignment(TextAlignment.Left)
                .FontSize(12)
                .Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dui leo, ultrices lobortis laoreet eu, auctor ac odio. Morbi mattis mi lectus. Mauris rhoncus risus at mollis malesuada. Proin neque dui, dignissim id ex eu, iaculis auctor orci. Duis efficitur eros in ante tincidunt tincidunt. In at nulla dictum, ultrices turpis convallis, vestibulum mi. Sed sit amet turpis vulputate, porta ante eget, lobortis arcu. Fusce aliquet ex iaculis sapien convallis tempor.Morbi a lacinia augue.Nam ligula dolor, vehicula nec augue in, mollis finibus magna.In posuere, arcu in pharetra sagittis, felis ex condimentum turpis, at malesuada tellus nisi id sem.Duis luctus, nunc quis accumsan tristique, turpis arcu consectetur velit, accumsan ornare felis neque vel dolor.Vestibulum placerat odio et pellentesque blandit.Suspendisse in augue id diam cursus condimentum vel vel justo.In viverra in sem eget bibendum.Fusce non maximus libero, id fermentum eros.Donec faucibus quis odio id luctus.Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.Nam dictum quam quis interdum feugiat.Aenean in ante dolor.")
                .Paragraph()
                .Add("Sub heading", fontSize: 14, fontWeight: 700, fontItalic: false)
                .Paragraph()
                .Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dui leo, ultrices lobortis laoreet eu, auctor ac odio. Morbi mattis mi lectus. Mauris rhoncus risus at mollis malesuada. Proin neque dui, dignissim id ex eu, iaculis auctor orci. Duis efficitur eros in ante tincidunt tincidunt. In at nulla dictum, ultrices turpis convallis, vestibulum mi. Sed sit amet turpis vulputate, porta ante eget, lobortis arcu. Fusce aliquet ex iaculis sapien convallis tempor.Morbi a lacinia augue.Nam ligula dolor, vehicula nec augue in, mollis finibus magna.In posuere, arcu in pharetra sagittis, felis ex condimentum turpis, at malesuada tellus nisi id sem.Duis luctus, nunc quis accumsan tristique, turpis arcu consectetur velit, accumsan ornare felis neque vel dolor.Vestibulum placerat odio et pellentesque blandit.Suspendisse in augue id diam cursus condimentum vel vel justo.In viverra in sem eget bibendum.Fusce non maximus libero, id fermentum eros.Donec faucibus quis odio id luctus.Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.Nam dictum quam quis interdum feugiat.Aenean in ante dolor.")
                .Paragraph()
                .Add("Sub heading", fontSize: 14, fontWeight: 700, fontItalic: false)
                .Paragraph()
                .Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dui leo, ultrices lobortis laoreet eu, auctor ac odio. Morbi mattis mi lectus. Mauris rhoncus risus at mollis malesuada. Proin neque dui, dignissim id ex eu, iaculis auctor orci. Duis efficitur eros in ante tincidunt tincidunt. In at nulla dictum, ultrices turpis convallis, vestibulum mi. Sed sit amet turpis vulputate, porta ante eget, lobortis arcu. Fusce aliquet ex iaculis sapien convallis tempor.Morbi a lacinia augue.Nam ligula dolor, vehicula nec augue in, mollis finibus magna.In posuere, arcu in pharetra sagittis, felis ex condimentum turpis, at malesuada tellus nisi id sem.Duis luctus, nunc quis accumsan tristique, turpis arcu consectetur velit, accumsan ornare felis neque vel dolor.Vestibulum placerat odio et pellentesque blandit.Suspendisse in augue id diam cursus condimentum vel vel justo.In viverra in sem eget bibendum.Fusce non maximus libero, id fermentum eros.Donec faucibus quis odio id luctus.Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.Nam dictum quam quis interdum feugiat.Aenean in ante dolor.")

                ;


            ScrollingTextArea area = new ScrollingTextArea(rs, 600, 300);

            var loc = new Vec3(0, 0.1f, -0.4f);

            Pose windowPoseButton = new Pose(loc, Quat.LookAt(loc, Input.Head.position));


            // Core application loop
            while (SK.Step(() =>
            {
                if (SK.System.displayType == Display.Opaque)
                    Default.MeshCube.Draw(floorMaterial, floorTransform);

                UI.WindowBegin("Window Button", ref windowPoseButton);

                area.AddScroller(windowPoseButton, 0.3f, 0.3f /2f);

                UI.WindowEnd();

                
            })) ;
            SK.Shutdown();
        }
    }
}
