Import ld

Function RectOverRect:Bool(tX1:Float, tY1:Float, tW1:Float, tH1:Float, tX2:Float, tY2:Float, tW2:Float, tH2:Float)
	If tX2 > tX1 + tW1 Then Return False
	If tX1 > tX2 + tW2 Then Return False
	If tY2 > tY1 + tH1 Then Return False
	If tY1 > tY2 + tH2 Then Return False
	Return True
End

Function PointInRect:Bool(X:Float, Y:Float, X1:Float, Y1:Float, W:Float, H:Float)
	If X < X1 Then Return False
	If Y < Y1 Then Return False
	If X > X1 + W Then Return False
	If Y > Y1 + H Then Return False
	Return True
End

Function CircleOverRect:Bool(X1:Float, Y1:Float, R1:Float, X2:Float, Y2:Float, W2:Float, H2:Float)
	If X1 + R1 < X2 Then Return False
	If X1 - R1 > X2 + W2 Then Return False
	If Y1 + R1 < Y2 Then Return False
	If Y1 - R1 > Y2 + H2 Then Return False
	Return True
	' THIS NEEDS MORE WORK FOR THE CORNER CHECKS
End

Function CircleOverCircle:Bool(X1:Float,Y1:Float,R1:Float,X2:Float,Y2:Float,R2:Float)
	If DistanceBetweenPoints(X1,Y1,X2,Y2) > (R1 + R2) Then Return False
	Return True
End

' Returns the direction in degrees between two points
Function DirectionBetweenPoints:Float(x1:Float,y1:Float,x2:Float,y2:Float,offset:Float = 0)
	Local dir:Float = ATan2((x2-x1),(y2-y1))
	If dir > 360
		dir -= 360
	End
	If dir < 0
		dir += 360
	End
	If offset <> 0
		dir = offset - dir
	End
	Return dir
End

Function DistanceBetweenPoints:Float(x1:Float,y1:Float,x2:Float,y2:Float)
	Local dX:Float = x2 - x1
	Local dY:Float = y2 - y1
	Return Sqrt((dX*dX)+(dY*dY))
End

