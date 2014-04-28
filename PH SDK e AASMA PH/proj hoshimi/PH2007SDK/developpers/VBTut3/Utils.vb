'Tutorial done by
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

    Public Shared Function Distance(ByVal ptA As Point, ByVal ptB As Point) As Integer
        Return (ptA.X - ptB.X) * (ptA.X - ptB.X) + (ptA.Y - ptB.Y) * (ptA.Y - ptB.Y)
    End Function

    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal entities As List(Of Entity)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each ent As Entity In entities
            Dim entPoint As New Point(ent.X, ent.Y)
            Dim entDistance As Integer = Distance(entPoint, currentLocation)
            If entDistance < dist Then
                dist = entDistance
                pReturn = entPoint
            End If
            Return pReturn
        Next
    End Function

    Public Shared Function getNearestPoint(ByVal currentLocation As Point, ByVal entities As List(Of Entity), ByVal exceptPoints As List(Of Point)) As Point
        Dim pReturn As Point = Point.Empty
        Dim dist As Integer = 200 * 200
        For Each ent As Entity In entities
            Dim entPoint As New Point(ent.X, ent.Y)
            Dim bExcept As Boolean
            If exceptPoints IsNot Nothing Then
                For Each exceptPoint As Point In exceptPoints
                    If entPoint = exceptPoint Then
                        bExcept = True
                        Exit For
                    End If
                Next

                If Not bExcept Then
                    Dim entDistance As Integer = MDistance(entPoint, currentLocation)
                    If entDistance < dist Then
                        dist = entDistance
                        pReturn = entPoint
                    End If
                End If
            End If

            Return pReturn
        Next
    End Function
End Class
