Import ld

Class TouchButton
	
	Field X:Int
	Field Y:Int
	Field W:Int
	Field H:Int
	
	Field Hit:Bool
	Field Down:Bool
	
	Field Active:Bool
	
	Method Activate:Void()
		Active = True
	End
	
	Method Deactivate:Void()
		Active = False
	End
	
	Method New(tX:Int, tY:Int, tW:Int, tH:Int)
		X = tX
		Y = tY
		W = tW
		H = tH
	End
	
	Method Check:Bool(tX:Int, tY:Int)
		Return PointInRect(tX, tY, X, Y, W, H)
	End
	
	Method Render:Void()
		SetColor(0, 0, 0)
		SetAlpha(0.25)
		DrawRect(X, Y, W, H)
		
		SetColor(255, 255, 255)
		SetAlpha(0.5)
		DrawHollowRect(X, Y, W, H)
	End

End