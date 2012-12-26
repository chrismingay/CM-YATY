Import ld

Class Yeti Extends Entity

	Global gfxStandFront:Image
	Global gfxRunFront:Image
	Global gfxRunLeft:Image
	Global gfxRunRight:Image
	
	Global gfxShadow:Image
	
	
	Global a:Yeti[]
	Global NextYeti:Int
	Const MAX_YETIS:Int = 1
	
	Function Init:Void(tLev:Level)
		a = New Yeti[MAX_YETIS]
		Entity.a[EntityType.YETI] = New Entity[MAX_YETIS]
		For Local i:Int = 0 Until MAX_YETIS
			a[i] = New Yeti(tLev)
			Entity.a[EntityType.YETI][i] = a[i]
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(48, 0, 22, 32, 2, Image.MidHandle)
		gfxRunFront = GFX.Tileset.GrabImage(48, 32, 22, 32, 2, Image.MidHandle)
		gfxRunLeft = GFX.Tileset.GrabImage(48, 64, 22, 32, 2, Image.MidHandle)
		gfxRunRight = GFX.Tileset.GrabImage(48, 96, 22, 32, 2, Image.MidHandle)
		
		gfxShadow = GFX.Tileset.GrabImage(112, 0, 16, 6, 1, Image.MidHandle)
		
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
	
	Field aniRunFrame:Int = 0
	Field aniRunFrameTimer:Float = 0.0
	Const ANI_RUN_FRAME_TIMER_TARGET:Float = 5.0
	
	Field aniWaitFrame:Int = 0
	Field aniWaitFrameTimer:Float = 0.0
	Const ANI_WAIT_FRAME_TIMER_TARGET:Float = 30.0
	
	Field Status:Int
	Field D:Int
	Field MaxYS:Float
	Field TargetXS:Float
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.YETI
		W = 20
		H = 30
	End
	
	Method Activate:Void()
		Super.Activate()
		Status = YetiStatusTypes.WATING_QUIET
	End
	
	Method Update:Void()
	
		Super.Update()
		
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
		
		Select Status
			Case YetiStatusTypes.CHASING
				UpdateChasing()
			Case YetiStatusTypes.DAZED
				UpdateDazed()
			Case YetiStatusTypes.EATING
				UpdateEating()
			Case YetiStatusTypes.WAITING_HAPPY
				UpdateWaitingHappy()
			Case YetiStatusTypes.WATING_QUIET
				UpdateWaiting()
		End
	
		If Not IsOnScreen()
			Deactivate()
		EndIf
	
	End
	
	Method StartWaiting:Void()
		Status = YetiStatusTypes.WATING_QUIET
	End
	
	Method UpdateWaiting:Void()
		If Controls.DownHit
			StartChasing()
		EndIf
	End
	
	Method StartChasing:Void()
		D = EntityMoveDirection.D
		XS = 0
		YS = 0.5
		ZS = -1
		Z = 0
		Status = YetiStatusTypes.CHASING
	End
	
	Method UpdateChasing:Void()
	
		UpdateControlled()
	
		If onFloor
			Select D
				Case EntityMoveDirection.L
					MaxYS = 0.0
					TargetXS = 0.0
				Case EntityMoveDirection.LLD
					MaxYS = 1.0
					TargetXS = -2.0
				Case EntityMoveDirection.LDD
					MaxYS = 2.0
					TargetXS = -1.0
				Case EntityMoveDirection.D
					MaxYS = 3.0
					TargetXS = 0.0
				Case EntityMoveDirection.RDD
					MaxYS = 2.0
					TargetXS = 1.0
				Case EntityMoveDirection.RRD
					MaxYS = 1.0
					TargetXS = 2.0
				Case EntityMoveDirection.R
					MaxYS = 0.0
					TargetXS = 0.0
			End
			
			If YS > MaxYS - 0.05 And YS < MaxYS + 0.05
				YS = MaxYS
			ElseIf YS > MaxYS
				Local tR:Float = 0.02
				If D = EntityMoveDirection.L Or D = EntityMoveDirection.R
					tR = 0.04
				EndIf
				YS *= (1.0 - (tR * LDApp.Delta))
			Else
				YS += (0.05 * LDApp.Delta)
			EndIf
			
			If XS > TargetXS - (0.05 * LDApp.Delta) And XS < TargetXS + (0.05 * LDApp.Delta)
				XS = TargetXS
			ElseIf XS < TargetXS
				XS += 0.05 * LDApp.Delta
			ElseIf XS > TargetXS
				XS -= 0.05 * LDApp.Delta
			EndIf
		Else
			ZS += (0.05 * LDApp.Delta)
		End
		
		X += XS * LDApp.Delta
		Y += YS * LDApp.Delta
		Z += ZS * LDApp.Delta
		
		
	End
	
	Method StartEating:Void()
		
	End
	
	Method UpdateEating:Void()
		
	End
	
	Method StartDazed:Void()
		
	End
	
	Method UpdateDazed:Void()
		
	End
	
	Method StartWaitingHappy:Void()
		
	End
	
	Method UpdateWaitingHappy:Void()
		
	End
	
	Method UpdateControlled:Void()
		If Controls.LeftHit
			If D > EntityMoveDirection.L
				D -= 1
			Else
				If onFloor
					XS = -1
				EndIf
			EndIf
		EndIf
		
		If Controls.RightHit
			If D < EntityMoveDirection.R
				D += 1
			Else
				If onFloor
					XS = 1
				EndIf
			EndIf
		EndIf
		
		If Controls.DownHit
			D = EntityMoveDirection.D
		EndIf
		
		If Controls.UpHit
			If onFloor
				ZS = -1.5
				XS *= 1.5
				YS *= 1.5
			EndIf
		EndIf
	End
	
	
	Method Render:Void()
	
		SetAlpha(0.25)
		GFX.Draw(gfxShadow, X, Y + 12)
		SetAlpha(1.0)
		
		Select Status
			Case YetiStatusTypes.CHASING
				RenderChasing()
			Case YetiStatusTypes.DAZED
				RenderDazed()
			Case YetiStatusTypes.EATING
				RenderEating()
			Case YetiStatusTypes.WAITING_HAPPY
				RenderWaitingHappy()
			Case YetiStatusTypes.WATING_QUIET
				RenderWaiting()
			Case YetiStatusTypes.POST_DIVE
		End
		
	End
	
	Method RenderWaiting:Void()
		GFX.Draw(gfxStandFront, X, Y, aniWaitFrame)
	End
	
	Method RenderChasing:Void()
		Select D
			Case EntityMoveDirection.L
				GFX.Draw(gfxStandFront, X, Y + Z, aniWaitFrame)
			Case EntityMoveDirection.LLD, EntityMoveDirection.LDD
				GFX.Draw(gfxRunLeft, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.D
				GFX.Draw(gfxRunFront, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.RDD, EntityMoveDirection.RRD
				GFX.Draw(gfxRunRight, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.R
				GFX.Draw(gfxStandFront, X, Y + Z, aniWaitFrame)
		End
	End
	
	Method RenderEating:Void()
		
	End
	
	Method RenderDazed:Void()
		
	End
	
	Method RenderWaitingHappy:Void()
		
	End

End

Class YetiStatusTypes
	Const WATING_QUIET:Int = 0
	Const CHASING:Int =1
	Const EATING:Int = 2
	Const DAZED:Int = 3
	Const WAITING_HAPPY:Int = 4
	Const POST_DIVE:Int = 5
End