Import ld

Class Level



	Field controlledYeti:Int = 0
	
	Global Width:Int = 1000
	Global Height:Int = 17500
	
	Field titleFade:Float = 0.0
	Field titleFadeMode:Int
	Field titleFadeTimer:Int
	
	Field txtWait:RazText
	
	Method New()
	
		Entity.Init()
		
		Dog.Init(LDApp.level)
		Yeti.Init(LDApp.level)
		Skier.Init(LDApp.level)
		Tree.Init(LDApp.level)
		Rock.Init(LDApp.level)
		Flag.Init(LDApp.level)
		
		controlledYeti = Yeti.Create(0, 0)
		Yeti.a[controlledYeti].StartWaiting()
		
		Dog.Create(50, 50)
		
		Local firstSkier:Int = Skier.Create(50, -200)
		Skier.a[firstSkier].TargetYeti = 0
		Skier.a[firstSkier].StartPreTeasing()
		
		
		LDApp.SetScreenPosition(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y)
		
		txtWait = New RazText()
		txtWait.AddMutliLines(LoadString("txt/wait.txt").Replace("~n", ""))
		txtWait.SetPos(96, 320)
		txtWait.SetSpacing(-3, -1)
		
		titleFade = 0.0
		titleFadeMode = 0
		titleFadeTimer = 0
		
	End
	
	Method Start:Void()
		SFX.Music("ambient")
	End

	Method Update:Void()
	
		LDApp.SetScreenTarget(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y + (LDApp.ScreenHeight * 0.15))
	
		Dog.UpdateAll()
		Yeti.UpdateAll()
		Skier.UpdateAll()
		Tree.UpdateAll()
		Rock.UpdateAll()
		
		UpdateTitleFade()
		
	End
	
	Method UpdateTitleFade:Void()
		Select titleFadeMode
			Case 0
				titleFadeTimer += 1
				If titleFadeTimer >= 120
					titleFadeTimer = 0
					titleFadeMode = 1
				EndIf
			Case 1
				titleFade += 0.02
				If titleFade >= 1
					titleFade = 1.0
					titleFadeMode = 2
				EndIf
			Case 2
				titleFadeTimer += 1
				If titleFadeTimer >= 240
					titleFadeTimer = 0
					titleFadeMode = 3
				EndIf
			Case 3
				titleFade -= 0.01
				If titleFade <= 0.0
					titleFade = 0.0
					titleFadeMode = 4
				EndIf
		End
	End
	
	Method RenderTitleFade:Void()
		If titleFadeMode >= 1 And titleFadeMode < 4
			SetAlpha(titleFade)
			DrawImage(GFX.Title, LDApp.ScreenWidth * 0.5, LDApp.ScreenHeight * 0.5)
			SetAlpha(1.0)
		EndIf
	End
	
	Method Render:Void()
	
		SetColor(0, 0, 0)
		SetAlpha(0.5)
		DrawImage(GFX.Overlay, 0, 0)
	
		
		SetColor(255, 255, 255)
		SetAlpha(1.0)
		
		RenderGui()
	
		Dog.RenderAll()
		Yeti.RenderAll()
		Skier.RenderAll()
		Tree.RenderAll()
		Rock.RenderAll()
		Flag.RenderAll()
		
		RenderTitleFade()
		' txtWait.Draw()
		
		
	End
	
	Method StartPreChase:Void()
		
	End
	
	Method UpdatePreChase:Void()
		
	End
	
	Method StartChase:Void()
		
	End
	
	Method UpdateChase:Void()
		
	End
	
	Method RenderGui:Void()
		SetColor(255, 255, 255)
		SetAlpha(1.0)
		DrawImageRect(GFX.Tileset, 1, 1, 504, 0, 8, 360)
		DrawImageRect(GFX.Tileset, 6, 1 + (Yeti.a[controlledYeti].Y / 50), 464, 0, 10, 10)
		For Local i:Int = 0 Until Skier.MAX_SKIERS
			If Skier.a[i].Active = True
				DrawImageRect(GFX.Tileset, 6, 1 + (Skier.a[i].Y / 50), 480, 0, 10, 10)
			EndIf
		Next
		
		
	End
	
End

Class LevelStatusType
	Const PRE_CHASE:Int = 0
	Const CHASING:Int = 1
	Const TOO_EARILY:Int = 2
End

Function GenerateLevel:Level()
	Local tL:Level = New Level()
	
	GenerateFlagTrail(tL)
	
	GenerateTrees(tL)
	
	GenerateRocks(tL)
	
	Return tL
End

Function GenerateFlagTrail:Void(tL:Level)
	
	Local tX:Int = 0
	Local subX:Int = 0
	Local tY:Int = 200
	Local tStep:Int = 200
	Local tLeft:Bool = False
	Local tWidth:Int = 50
	For Local i:Int = 0 Until Flag.MAX_FLAGS
		tX = Sin(tY) * tWidth
		subX = Sin(tY * 3) * tWidth * 0.1
		
		Local tF:Int = Flag.Create(tX + subX, tY)
		';Local tF:Int = Flag.Create(0, tY)
		
		
		If tLeft
			tLeft = False
			Flag.a[tF].Type = FlagType.LEFT_SIDE
		Else
			tLeft = True
			Flag.a[tF].Type = FlagType.RIGHT_SIDE
		EndIf
		
		tY += tStep
		
	Next


End

Function GenerateTrees:Void(tL:Level)
	For Local i:Int = 0 Until Tree.MAX_TREES
		Local tX:Int = Rnd(0 - (Level.Width * 0.5), (Level.Width * 0.5))
		Local tY:Int = Rnd(0, (Level.Height))
		Tree.Create(tX,tY)
	Next
End

Function GenerateRocks:Void(tL:Level)
	For Local i:Int = 0 Until Rock.MAX_ROCKS
	
		Local tGood:Bool = False
		Local tX:Int
		Local tY:Int
		
		While tGood = False
			tX = Rnd(0 - (Level.Width * 0.5), (Level.Width * 0.5))
			tY = Rnd(0, (Level.Height))
			
			If tY Mod 4000 > 2000
				tGood = True
			EndIf
		Wend
		
		Rock.Create(tX, tY)
	Next
End

Function GenerateBumps:Void(tL:Level)
	For Local i:Int = 0 Until Bump.MAX_BUMPS
	
		Local tX:Int
		Local tY:Int
		
		tX = Rnd(0 - (Level.Width * 0.5), (Level.Width * 0.5))
		tY = Rnd(0, (Level.Height))
		
		
		Bump.Create(tX, tY)
	Next
End