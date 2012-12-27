Import ld

Class Yeti Extends Entity

	Global gfxStandFront:Image
	Global gfxRunFront:Image
	Global gfxRunLeft:Image
	Global gfxRunRight:Image
	
	Global gfxStopLeft:Image
	Global gfxStopRight:Image
	
	Global gfxEating:Image
	
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
		
		gfxStopLeft = GFX.Tileset.GrabImage(92, 32, 22, 32, 1, Image.MidHandle)
		gfxStopRight = GFX.Tileset.GrabImage(114, 32, 22, 32, 1, Image.MidHandle)
		
		gfxEating = GFX.Tileset.GrabImage(96, 96, 32, 32, 6, Image.MidHandle)
		
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
		a[thisYeti].Collidable = False
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
	
	Field stepTimer:Float
	Field stepTimerThreshold:Float = 25.0
	
	Field aniEatingFrame:Int
	Field aniEatingFrameTimer:Float = 0.0
	Const ANI_EATING_FRAME_TIMER_TARGET:Float = 6.0
	Field aniEatingGeneralTimer:Int = 0
	
	Field aniHappyFrame:Int = 0
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.YETI
		W = 12
		H = 26
	End
	
	Method Activate:Void()
		Super.Activate()
		Status = YetiStatusTypes.WATING_QUIET
	End
	
	Method Update:Void()
	
		Super.Update()
		
		If onFloor
			stepTimer += (Abs(XS) + Abs(YS)) * LDApp.Delta
			If stepTimer >= stepTimerThreshold
				stepTimer = 0.0
				Local tR:Float = Rnd()
				If tR < 0.25
					SFX.Play("SnowStep1", Rnd(0.5, 0.7), 0.0, Rnd(0.8, 1.2))
				ElseIf tR < 0.5
					SFX.Play("SnowStep2", Rnd(0.5, 0.7), 0.0, Rnd(0.8, 1.2))
				ElseIf tR < 0.75
					SFX.Play("SnowStep3", Rnd(0.5, 0.7), 0.0, Rnd(0.8, 1.2))
				Else
					SFX.Play("SnowStep4", Rnd(0.5, 0.7), 0.0, Rnd(0.8, 1.2))
				EndIf
				
			EndIf
		EndIf
		
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
		If Controls.DownHit And Skier.a[0].Status = SkierStatusType.SKIING
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
		
		Local colStatus:Int = CheckForCollision()
		
		If colStatus <> EntityType.NONE
		
			If colStatus <> EntityType.SKIER
				Local tR:Float = Rnd()
				If tR < 0.25
					SFX.Play("YetiHit1", Rnd(0.5, 0.7), 0.0, Rnd(0.95, 1.05))
				ElseIf tR < 0.5
					SFX.Play("YetiHit2", Rnd(0.5, 0.7), 0.0, Rnd(0.95, 1.05))
				ElseIf tR < 0.75
					SFX.Play("YetiHit3", Rnd(0.5, 0.7), 0.0, Rnd(0.95, 1.05))
				Else
					SFX.Play("YetiHit4", Rnd(0.5, 0.7), 0.0, Rnd(0.95, 1.05))
				EndIf
			EndIf
		EndIf
		
		Select colStatus
		Case EntityType.BUMP
			ZS = -1
		Case EntityType.DOG
			ZS = -1
			XS *= 0.8
			YS *= 0.8
		Case EntityType.FLAG
			XS *= 0.8
			YS *= 0.8
		Case EntityType.JUMP
			XS *= 2
			YS *= 2
			ZS = -2
		Case EntityType.ROCK
			XS *= 0.4
			YS = 0 - (YS * 0.5)
		Case EntityType.SNOWBOARDER
		
		Case EntityType.SKIER
			If Skier.a[0].onFloor And onFloor
				Skier.a[0].Deactivate()
				StartEating()
			EndIf
		Case EntityType.TREE
			XS *= 0.6
			YS *= 0.2
		Case EntityType.YETI
			' TODO END GAME
		End
		
		
	End
	
	Method StartEating:Void()
		Status = YetiStatusTypes.EATING
		aniEatingFrame = 0
		aniEatingFrameTimer = 0.0
		
		XS = 0
		YS = 0
		Z = 0
		ZS = 0
	End
	
	Method UpdateEating:Void()
		aniEatingFrameTimer += 1.0 * LDApp.Delta
		If aniEatingFrameTimer >= ANI_EATING_FRAME_TIMER_TARGET
			aniEatingFrameTimer = 0.0
			aniEatingFrame += 1
			If aniEatingFrame = 6
				aniEatingFrame = 4
			EndIf
		EndIf
		
		aniEatingGeneralTimer += 1
		
		If aniEatingGeneralTimer = 120
			StartWaitingHappy()
		EndIf
		
	
	End
	
	Method StartDazed:Void()
		
	End
	
	Method UpdateDazed:Void()
		
	End
	
	Method StartWaitingHappy:Void()
		Status = YetiStatusTypes.WAITING_HAPPY
		ZS = 0 - Rnd(1, 2)
		aniHappyFrame = 0
	End
	
	Method UpdateWaitingHappy:Void()
	
		Z += ZS * LDApp.Delta
		
		ZS += (0.1 * LDApp.Delta)
	
		If Z > 0
			ZS = 0 - Rnd(1, 2)
			Z = 0
			aniHappyFrame += 1
			If aniHappyFrame = 2
				aniHappyFrame = 0
			EndIf
		EndIf
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
		
	End
	
	
	Method Render:Void()
	
		Super.Render()
	
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
				GFX.Draw(gfxStopLeft, X, Y + Z)
			Case EntityMoveDirection.LLD, EntityMoveDirection.LDD
				GFX.Draw(gfxRunLeft, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.D
				GFX.Draw(gfxRunFront, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.RDD, EntityMoveDirection.RRD
				GFX.Draw(gfxRunRight, X, Y + Z, aniRunFrame)
			Case EntityMoveDirection.R
				GFX.Draw(gfxStopRight, X, Y + Z)
		End
	End
	
	Method RenderEating:Void()
		GFX.Draw(gfxEating, X + 3, Y - 1 + Z, aniEatingFrame)
	End
	
	Method RenderDazed:Void()
		
	End
	
	Method RenderWaitingHappy:Void()
		GFX.Draw(gfxRunFront, X, Y + Z, aniHappyFrame)
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