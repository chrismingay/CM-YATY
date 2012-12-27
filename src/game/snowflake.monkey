Import ld

Class Snowflake

	Const MAX_SNOWFLAKES:Int = 200
	Global a:Snowflake[]
	Global NextSnowFlake:Int = 0

	Global gfxSnow:Image
	
	Function Init:Void()
		gfxSnow = GFX.Tileset.GrabImage(0, 368, 8, 8, 3, Image.MidHandle)
		a = New Snowflake[MAX_SNOWFLAKES]
		For Local i:Int = 0 Until MAX_SNOWFLAKES
			a[i] = New Snowflake()
		Next
		NextSnowFlake = 0
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_SNOWFLAKES
			If a[i].Active
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_SNOWFLAKES
			If a[i].Active
				a[i].Render()
			End
		Next
	End
	
	Function Create:Void(tX:Float, tY:Float)
		a[NextSnowFlake].X = tX
		a[NextSnowFlake].Y = tY
		
		a[NextSnowFlake].XS = Rnd(-1, 2)
		a[NextSnowFlake].YS = Rnd(0.1,1.0)
		
		a[NextSnowFlake].Active = True
		
		a[NextSnowFlake].Frame = Rnd(0.0, 3.0)
		
		NextSnowFlake += 1
		If NextSnowFlake >= MAX_SNOWFLAKES
			NextSnowFlake = 0
		EndIf
	End
	

	Const DRAW_X:Int = 0
	Const DRAW_Y:Int = 0

	Const WIDTH:Int = 16
	Const HEIGHT:Int = 16
	
	Field X:Float
	Field Y:Float
	Field XS:Float
	Field YS:Float
	
	Field Active:Bool
	
	Field Frame:Int
	
	Method New()
		Active = False
	End
	
	Method Update:Void()
	
		X += XS
		Y += YS
		
		'XS += Rnd(-0.1, 0.1)
		
		XS = Sin(Y)
		
		'YS += Rnd(-0.1,0.1)
		If YS < 0
			YS = 0
		End
	
		If X > LDApp.ScreenWidth + WIDTH Or Y > LDApp.ScreenHeight + HEIGHT
			Active = False
		End
			
	End
	
	Method Render:Void()
		GFX.Draw(gfxSnow, X, Y, Frame, False)
	End
	
End
	