'Template done by
' 
'Richard Clark
'Project Hoshimi Lead Manager
'Contact at ori@c2i.fr
' 
Imports PH.Common
Imports PH.Map
Imports System.Drawing

Public Class Utils
    Public Shared Function MDistance(ByVal p1 As Point, ByVal p2 As Point) As Integer
        Return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y)
    End Function

    Public Shared Function SquareDistance(ByVal ptA As Point, ByVal ptB As Point) As Integer
        Return (ptA.X - ptB.X) * (ptA.X - ptB.X) + (ptA.Y - ptB.Y) * (ptA.Y - ptB.Y)
    End Function

    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal entities As List(Of Entity)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each ent As Entity In entities
            Dim entPoint As New Point(ent.X, ent.Y)
            Dim entDistance As Integer = SquareDistance(entPoint, currentLocation)
            If entDistance < dist Then
                dist = entDistance
                pReturn = entPoint
            End If
        Next
        Return pReturn
    End Function
    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal points As List(Of Point)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each p As Point In points
            Dim entDistance As Integer = SquareDistance(p, currentLocation)
            If entDistance < dist Then
                dist = entDistance
                pReturn = p
            End If
        Next
        Return pReturn
    End Function
    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal entities As List(Of Entity), ByVal exceptPoints As List(Of Point)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each ent As Entity In entities
            Dim entPoint As New Point(ent.X, ent.Y)
            Dim bExcept As Boolean = False
            If exceptPoints IsNot Nothing Then
                For Each exceptPoint As Point In exceptPoints
                    If entPoint = exceptPoint Then
                        bExcept = True
                        Exit For
                    End If
                Next
            End If
            If Not bExcept Then
                Dim entDistance As Integer = MDistance(entPoint, currentLocation)
                If entDistance < dist Then
                    dist = entDistance
                    pReturn = entPoint
                End If
            End If
        Next
        Return pReturn
    End Function
    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal availablePoints As List(Of Point), ByVal exceptPoints As List(Of Point)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each p As Point In availablePoints
            Dim bExcept As Boolean = False
            If exceptPoints IsNot Nothing Then
                For Each exceptPoint As Point In exceptPoints
                    If p = exceptPoint Then
                        bExcept = True
                        Exit For
                    End If
                Next
            End If
            If Not bExcept Then
                Dim entDistance As Integer = MDistance(p, currentLocation)
                If entDistance < dist Then
                    dist = entDistance
                    pReturn = p
                End If
            End If
        Next
        Return pReturn
    End Function

    Public Shared Function getMiddlePoint(ByVal points As Point()) As Point
        If points Is Nothing OrElse points.Length = 0 Then Return Point.Empty

        Dim sumX As Integer = 0
        Dim sumY As Integer = 0
        For Each p As Point In points
            sumX += p.X
            sumY += p.Y
        Next

        Dim x As Integer = CType(Math.Round(1.0F * sumX / points.Length), Integer)
        Dim y As Integer = CType(Math.Round(1.0F * sumY / points.Length), Integer)

        Return New Point(x, y)
    End Function

    Public Shared Function getValidPoint(ByVal tissue As Tissue, ByVal p As Point) As Point
        If (IsPointOK(tissue, p.X, p.Y)) Then Return p
        Dim dist As Integer = 1
        Do
            'up
            For iX As Integer = -dist To dist
                If (IsPointOK(tissue, p.X + iX, p.Y + dist)) Then _
                        Return New Point(p.X + iX, p.Y + dist)
            Next
            'down
            For iX As Integer = -dist To dist
                If (IsPointOK(tissue, p.X + iX, p.Y - dist)) Then _
                    Return New Point(p.X + iX, p.Y - dist)
            Next
            'left
            For iY As Integer = -dist To dist
                If (IsPointOK(tissue, p.X - dist, p.Y + iY)) Then _
                Return New Point(p.X - dist, p.Y + iY)
            Next
            'right
            For iY As Integer = -dist To dist
                If (IsPointOK(tissue, p.X + dist, p.Y + iY)) Then _
            Return New Point(p.X + dist, p.Y + iY)
            Next
            dist += 1
        Loop
    End Function

    Private Shared Function IsPointOK(ByVal tissue As Tissue, ByVal X As Integer, ByVal Y As Integer) As Boolean
        If (Not tissue.IsInMap(X, Y)) Then Return False

        Return tissue(X, Y).AreaType = AreaEnum.HighDensity OrElse _
            tissue(X, Y).AreaType = AreaEnum.MediumDensity OrElse _
            tissue(X, Y).AreaType = AreaEnum.LowDensity
    End Function
End Class
