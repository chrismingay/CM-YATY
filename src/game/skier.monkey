Import ld

Class Skier Extends Entity

	Global gfxSkiL:Image
	Global gfxSkiLLD:Image
	Global gfxSkiLDD:Image
	Global gfxSkiD:Image
	Global gfxSkiRDD:Image
	Global gfxSkiRRD:Image
	Global gfxSkiR:Image
	
	
	Global a:Skier[]
	Const MAX_SKIERS:Int = 20
	Global NextSkier:Int
	
	
	Function Init:Void(tLev:Level)
		a = New Skier[MAX_SKIERS]
		For Local i:Int = 0 Until MAX_SKIERS
			a[i] = New Skier(tLev)
		Next
		
		gfxSkiL = GFX.Tileset.GrabImage(70, 128, 22, 32, 1, Image.MidHandle)
		gfxSkiLLD = gfxSkiL
		gfxSkiLDD = gfxSkiL
		gfxSkiD = gfxSkiL
		gfxSkiRDD = gfxSkiL
		gfxSkiRRD = gfxSkiL
		gfxSkiR = gfxSkiL
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_SKIERS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Update()
				EndIf
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_SKIERS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				Else
					a[i].RenderMarker()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tI:Int = NextSkier
		a[tI].Activate()
		a[tI].SetPosition(tX, tY)
		
		NextSkier += 1
		If NextSkier >= MAX_SKIERS
			NextSkier = 0
		EndIf
		
		Return tI
		
	End
	
	Field D:Int
	Field Status:Int
	
	Field MaxYS:Float
	Field TargetXS:Float
	
	Field TargetYeti:Int
	
	Const MAX_YS:Float = 10.0
	
	Method New(tLev:Level)
		level = tLev
	End
	
	Method Update:Void()
	
		Select Status
		Case SkierStatusType.DAZED
			UpdateDazed()
		Case SkierStatusType.DEAD
			UpdateDead()
		Case SkierStatusType.FALLING
			UpdateFalling()
		Case SkierStatusType.SKIING
			UpdateSkiing()
		Case SkierStatusType.TEASING
			UpdateTeasing()
		End
	
		If Not IsOnScreen()
			Deactivate()
		EndIf
	End
	
	Method StartSkiing:Void()
	
		Status = SkierStatusType.SKIING
		Z = 0
		D = EntityMoveDirection.D
		YS = 3.0
		XS = 0
		ZS = -2.0
	
	End
	
	Method UpdateSkiing:Void()
	
		D = Clamp(D, EntityMoveDirection.L, EntityMoveDirection.R)
		
		If Rnd() < 0.02
			If Rnd() < 0.2
				D = EntityMoveDirection.D
			Else
				If Rnd() < 0.5
					D -= 1
				Else
					D += 1
				EndIf
			EndIf
		EndIf
	
	
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
			
			If YS > MaxYS
				YS *= (1.0 - (0.05 * LDApp.Delta))
			Else
				YS *= (1.0 + (0.05 * LDApp.Delta))
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
	
	Method StartFalling:Void()
		Local tZ:Float = (Abs(XS) + Abs(YS)) * 0.5
		ZS = 0 - tZ
	End
	
	Method UpdateFalling:Void()
		
	End
	
	Method StartDazed:Void()
		
	End
	
	Method UpdateDazed:Void()
		
	End
	
	Method StartDead:Void()
		
	End
	
	Method UpdateDead:Void()
		
	End
	
	Method StartTeasing:Void()
		Status = SkierStatusType.TEASING
		D = EntityMoveDirection.D
		XS = 0
		YS = 0
		TargetYeti = FindNearestYeti()
	End
	
	Method UpdateTeasing:Void()
		If Rnd() < 0.05
			Y += 1
		EndIf
		
		If Yeti.a[TargetYeti].Y - Y < 2
			StartSkiing()
		ElseIf Yeti.a[TargetYeti].Y - Y < 50
			If Rnd() < 0.02
				StartSkiing()
			EndIf
		EndIf
	End
	
	Method Render:Void()
		GFX.Draw(gfxSkiL, X, Y + Z, 0)
	End
	
	Method RenderMarker:Void()
		If X < LDApp.ScreenX
			DrawImageRect(GFX.Tileset, 0, 300, 0, 176, 8, 9)
		ElseIf X > LDApp.ScreenX + LDApp.ScreenWidth
			DrawImageRect(GFX.Tileset, 232, 300, 8, 176, 8, 9)
		EndIf
	End
	
	Method Activate:Void()
		Super.Activate()
		TargetYeti = -1
	End
	
	Method FindNearestYeti:Int()
		Local nIndex:Int = -1
		Local nDistance:Float = 99999999
		
		For Local i:Int = 0 Until Yeti.MAX_YETIS
			If Yeti.a[i].Active = True
				If Yeti.a[i].Y > Y
					Local tDist:Float = Yeti.a[i].Y - Y
					If tDist < 200
						If tDist < nDistance
							nIndex = i
							nDistance = tDist
						End
					End
				EndIf
			EndIf
		Next
		
		Return nIndex
	End
	

End

Class SkierStatusType
	Const SKIING:Int = 0
	Const FALLING:Int = 1
	Const DAZED:Int = 2
	Const DEAD:Int = 3
	Const TEASING:Int = 4
End