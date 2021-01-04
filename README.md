
2020-jan-03-02:14am - 
i added the SCCoreSystemsMono solution. in that solution, the virtual reality rendering to the monogame directX window isn't working yet.
The SCMonoAB3DVR solution is displayed both in the headset and in the monogame directX window. I will see if i can make the jitter physics engine work correctly later. I removed the ab3d.dxengine nugget and the ab3d.dxengine.oculusWrap built dlls. If you want the better quality, please follow the steps i posted on the 02 jan 2021.

thank you for reading me,
steve chassé

2021-jan-02-
i wanted this to have the 60 days trial using the ab3d.dxengine.OculusWrap, right out of the box, for people to use the ab3d.dxengine.oculusWrap. I modified my repos so you won't be able to make them work out of the box anymore.

You will have to go clone the repository here https://github.com/ab4d/Ab3d.OculusWrap and build the dlls separately for yourselves. If the ab3d.dxengine.oculusWrap would be provided in the future with a nugget you won't have to do those steps. 

1. Clone the github repository here: https://github.com/ab4d/Ab3d.OculusWrap
2. Build the ab3d.OculusWrap solution first with the FrameWork 4.5 or 4.7.2 whatever.
3. Then build the solution ab3d.DXEngine.OculusWrap.
4. use both the ab3d.OculusWrap.dll and the ab3d.DXEngine.OculusWrap as references for my projects sc_core_systems and SCCoreSystems and the solution sccsv10 and this one sccsv11 and that one too sccsVD4VE. Those DLLs will make the projects work. after inserting those references, rebuild your projects and this should take care of restoring the nugget packages for the other dlls to load.

thank you for reading me,
steve chassé

2020-dec-25-
OCULUS RIFT CV1 ONLY
# SCCoreSystemsMono
Ab3d OVR wrapper Monogame with jitter physics that i built myself. Something is not working anymore with the Content Pipeline for the original Jitter car.fbx so i am going to work on it soon. 

i dicked around a bit in trying to make a voxel planet and after a windows 10 computer reset, the car.fbx file isn't loading up correctly as a model. i will try and work on this soon. So the first demo has an unfinished voxel spheroid chunk spawn right there in the scene... It breaks the point of a working jitter physics demo to not be able to instantiate any objects either and that the terrain is invisible... coming with some fixes soon.

once the program starts, you can press the minus keyboard key sign or the plus keyboard key sign in order to change the demo, but the second demo with the car.fbx file is no longer working after a microsoft windows 10 reset for elite dangerous oculus touch no longer working when steamVR was installed. since then, the car.fbx isn't working in visual studio with monogame 3.7.1.189 with dotnet 3.1 installed with visual studio 2017 or 2019. i will do some more tests tonight and also remove my simple chunk tests in monogame, that i hadn't tested before. The ab3d.dxengine.oculuswrap is working here and somehow it's instantly compatible with the rendertarget2D of monogame microsoft.framework graphics. Oculus Rift CV1 headset. It works as you can see the 3d scene through the oculus headset but i cannot do both headset/monogameDirectXWindow yet. But the rendertarget2d of the monogame graphics windows isn't displaying it. So inside the headset, you see the virtual reality jitter physics engine active, but it is not currently rendering in the monogame directx window draw function yet. The reason is i wasn't able to build monogame 3.7.1.189 from source and have a couple of options implemented for the draw function. From the start, the monogame graphics 3.7.1.189 framework and it's grip on the directx window is very hard to ungrip/unhook... i didnt find any other solution but to build from source and change a single function for that problem. 

i have a version of the virtual desktop for the ab3d.dxengine and the ab3d.dxengine.oculuswrap, but i told andrej benedik that i was giving him the project after some point, because i wanted to dig deeper into using only the sharpDX dlls as per the rastertek tutorials translated from c++ to c# by github user Dan6040. My program is a simple incomplete program at an attempt to mix instanced drawing on screen but each instanced polygon or object uses the physics engine by receiving the estimated forward/right/up vector from the physics engine quaternion and also the position of the rigidbody. It renders fast the instanced objects, and also i have tried to use an instanced jitter physics engine. I have made tons of tests to see if i could make the jitter physics engine any faster but it ended up being total junk so you will find tons of commented code in my jitter_sc.cs script. I asked andrej if i could at least release the same version i had sent to him to at least show my progress in a github portfolio and he accepted. I am very happy about that. But im gonna have to find a solution as my virtuals desktops versions 0.38 up to 0.41 arent working with the ab3d.dxengine versions higher than 2.3... those 0.38 to 0.41 versions are old stuff but my virtual reality screen is still the same image that is coming from the xoofx sharpdx library capture screen... im just getting the texture2d and putting it straight on a rectangle object of the same resolution as my screen... its easy to do, anybody can do that. 

https://github.com/Dan6040/SharpDX-Rastertek-Tutorials
https://archive.codeplex.com/?p=rastertekdx
http://www.rastertek.com/tutdx11.html

I have seen others use the same principle in their shaders in order to have the cpu physics engine position and be shared to the HLSL shader. I will review my twitch streams and obs screen recordings to see when i came up with that, although others might have done it prior to me. i use that principle of instanced objects using the physics engine position/rotation sent to the shader, in one of those versions of the monogame/jitter program i have coded in my free time.

I will keep working on this jitter physics with the Oculus Rift CV1 with the ab3d.dxengine and ab3d.dxengine.oculuswrap when i have the time. this project uses the trial version 60 days of the ab3d.dxengine, and after 60 days, you will not be able to use it unless you purchase a license here https://www.ab4d.com/Purchase.aspx#DXEngine. 

https://github.com/ab4d/Ab3d.OculusWrap/tree/master/Ab3d.OculusWrap/Ab3d.OculusWrap.DemoDX11

I will keep trying and see if i can build MonoGame 3.7.1.189 from source and update this repo if i ever am able to code it soon.

thank you for reading me.
steve chassé








