Import ld

Class Yeti Extends Entity

	Global gfxStandFront:Image
	Global gfxRunFront:Image
	Global gfxRunLeft:Image
	Global gfxRunRight:Image
	
	
	Field aniRunFrame:Int = 0
	Field aniRunFrameTimer:Float = 0.0
	Const ANI_RUN_FRAME_TIMER_TARGET:Float = 5.0
	
	Field aniWaitFrame:Int = 0
	Field aniWaitFrameTimer:Float = 0.0
	Const ANI_WAIT_FRAME_TIMER_TARGET:Float = 30.0
	


	Global a:Yeti[]
	Global NextYeti:Int
	Const MAX_YETIS:Int = 1
	
	Function Init:Void(tLev:Level)
		a = New Yeti[MAX_YETIS]
		For Local i:Int = 0 Until MAX_YETIS
			a[i] = New Yeti(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(48, 0, 22, 32, 2, Image.MidHandle)
		gfxRunFront = GFX.Tileset.GrabImage(48, 32, 22, 32, 2, Image.MidHandle)
		gfxRunLeft = GFX.Tileset.GrabImage(48, 64, 22, 32, 2, Image.MidHandle)
		gfxRunRight = GFX.Tileset.GrabImage(48, 96, 22, 32, 2, Image.MidHandle)
		
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_YETIS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_YETIS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local thisYeti:Int = NextYeti
		a[thisYeti].Activate()
		NextYeti += 1
		If NextYeti = MAX_YETIS
			NextYeti = 0
		EndIf
		Return thisYeti
	End
	
	Method New(tLev:Level)
		level = tLev
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
		
		aniRunFrameTimer += 1.0 * LDApp.Delta
		If aniRunFrameTimer >= ANI_RUN_FRAME_TIMER_TARGET
			aniRunFrameTimer = 0.0
			aniRunFrame += 1
			If aniRunFrame >= 2
				aniRunFrame = 0
			EndIf
		EndIf
		
		aniWaitFrameTimer += 1.0 * LDApp.Delta
		If aniWaitFrameTimer >= ANI_WAIT_FRAME_TIMER_TARGET
			aniWaitFrameTimer = 0.0
			aniWaitFrame += 1
			If aniWaitFrame >= 2
				aniWaitFrame = 0
			EndIf
		EndIf
	
		If Not IsOnScreen()
			Deactivate()
		EndIf
	
	End
	
	Method Render:Void()
		
		If Controls.ActionDown
			If Controls.LeftDown
				GFX.Draw(gfxRunLeft, X, Y, aniRunFrame)
			ElseIf Controls.RightDown
				GFX.Draw(gfxRunRight, X, Y, aniRunFrame)
			Else
				GFX.Draw(gfxRunFront, X, Y, aniRunFrame)
			EndIf
			
		Else
			GFX.Draw(gfxStandFront, X, Y, aniWaitFrame)
		End
	End

End