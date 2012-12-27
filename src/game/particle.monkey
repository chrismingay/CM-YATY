Import ld

Class Particle

	Const DRAW_X:Int = 192
	Const DRAW_Y:Int = 48
	Const WIDTH:Int = 16
	Const HEIGHT:Int = 16

	Global Particles:Particle[]
	Const MAX_PARTICLES:Int = 200
	Global NextParticle:Int = 0
	
	Global gfxWood:Image
	Global gfxLeaf:Image
	Global gfxRock:Image
	Global gfxPole:Image
	Global gfxBlueFlag:Image
	Global gfxRedFlag:Image
	Global gfxSnow:Image
	Global gfxJump:Image
	Global gfxShadow:Image
	
	Function Init:Void(tLev:Level)
		Particles = New Particle[MAX_PARTICLES]
		For Local i:Int = 0 Until MAX_PARTICLES
			Particles[i] = New Particle(tLev)
		Next
		
		gfxWood = GFX.Tileset.GrabImage(0, 304, 8, 8, 4, Image.MidHandle)
		gfxRock = GFX.Tileset.GrabImage(0, 312, 8, 8, 4, Image.MidHandle)
		gfxLeaf = GFX.Tileset.GrabImage(0, 320, 8, 8, 4, Image.MidHandle)
		gfxPole = GFX.Tileset.GrabImage(0, 328, 8, 8, 4, Image.MidHandle)
		gfxBlueFlag = GFX.Tileset.GrabImage(0, 336, 8, 8, 4, Image.MidHandle)
		gfxRedFlag = GFX.Tileset.GrabImage(0, 344, 8, 8, 4, Image.MidHandle)
		gfxSnow = GFX.Tileset.GrabImage(0, 352, 8, 8, 4, Image.MidHandle)
		gfxJump = GFX.Tileset.GrabImage(0, 360, 8, 8, 4, Image.MidHandle)
		gfxShadow = GFX.Tileset.GrabImage(32, 304, 8, 8, 4, Image.MidHandle)
	End
	
	Function Add:Void(tX:Float, tY:Float, tXS:Float, tYS:Float, tType:Int)
		Particles[NextParticle].Activate(tX, tY, tXS, tYS, tType)
		NextParticle += 1
		If NextParticle >= MAX_PARTICLES
			NextParticle = 0
		EndIf
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_PARTICLES
			If Particles[i].Active = True
				Particles[i].Update()
			EndIf
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_PARTICLES
			If Particles[i].Active = True
				If Particles[i].IsOnScreen()
					Particles[i].Render()
				EndIf
			EndIf
		Next
	End
	
	Field X:Float
	Field Y:Float
	Field Z:Float
	Field XS:Float
	Field YS:Float
	Field ZS:Float
	
	Field Type:Int
	
	Field Active:Bool
	
	'Field lifeSpan:Float
	
	Field level:Level
	
	Field Frame:Int
	Field FrameTimer:Float
	
	Const FRAME_TIMER_LIMIT:Float = 10.0
	
	Method New(tlev:Level)
		level = tlev
		Active = False
	End
	
	Method Update:Void()
	
		X += (XS * LDApp.Delta)
		Y += (YS * LDApp.Delta)
		Z += (ZS * LDApp.Delta)
	
		'lifeSpan -= 1.0 * LDApp.Delta
		'If lifeSpan <= 0
		'	Deactivate()
		'EndIf
		
		FrameTimer += 1.0 * LDApp.Delta
		If FrameTimer >= FRAME_TIMER_LIMIT
			FrameTimer = 0
			Frame += 1
			If Frame = 4
				Frame = 0
			EndIf
		EndIf
		
		ZS += (0.02 * LDApp.Delta)
		
		If Z > 0
			Z = 0
			If ZS > 0.5
				ZS = 0 - (ZS * 0.5)
				XS *= 0.75
				YS *= 0.75
			Else
				Deactivate()
			End
		EndIf
	End
	
	Method Render:Void()
	
		GFX.Draw(gfxShadow, X, Y)
	
		Select Type
			Case ParticleTypes.LEAF
				GFX.Draw(gfxLeaf, X, Y + Z, Frame)
			Case ParticleTypes.WOOD
				GFX.Draw(gfxWood, X, Y + Z, Frame)
			Case ParticleTypes.ROCK
				GFX.Draw(gfxRock, X, Y + Z, Frame)
			Case ParticleTypes.POLE
				GFX.Draw(gfxPole, X, Y + Z, Frame)
			Case ParticleTypes.BLUE_FLAG
				GFX.Draw(gfxBlueFlag, X, Y + Z, Frame)
			Case ParticleTypes.RED_FLAG
				GFX.Draw(gfxRedFlag, X, Y + Z, Frame)
			Case ParticleTypes.SNOW
				GFX.Draw(gfxSnow, X, Y + Z, Frame)
			Case ParticleTypes.JUMP
				GFX.Draw(gfxJump, X, Y + Z, Frame)
		End
	End
	
	Method Deactivate:Void()
		Active = False
	End
	
	Method Activate(tX:Float, tY:Float, tXS:Float, tYS:Float, tType:Int)
		Active = True
		X = tX
		Y = tY
		XS = tXS
		YS = tYS
		Z = -0.1
		ZS = -1
		Type = tType
		
	End
	
	Method IsOnScreen:Bool()
		Return RectOverRect(X - (WIDTH * 0.5), Y - (HEIGHT * 0.5), WIDTH, HEIGHT, LDApp.ScreenX, LDApp.ScreenY, LDApp.ScreenWidth, LDApp.ScreenHeight)
	End

End

Class ParticleTypes
	Const WOOD:Int = 0
	Const ROCK:Int = 1
	Const LEAF:Int = 2
	Const POLE:Int = 3
	Const BLUE_FLAG:Int = 4
	Const RED_FLAG:Int = 5
	Const SNOW:Int = 6
	Const JUMP:Int = 7
End