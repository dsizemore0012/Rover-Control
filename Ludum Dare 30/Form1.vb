Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.GamerServices
Imports Microsoft.Xna.Framework.Media


Public Class Form1

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
End Class
Public Class Game
    Inherits Microsoft.Xna.Framework.Game
    'Fields in our game graphic manager etc'
    Dim graphics As GraphicsDeviceManager
    Dim spriteBatch As SpriteBatch
    Dim Rover As New character
    Dim MenuRover As New character
    Dim BrickTemplate As New Wall
    Dim FloorTemplate As New Wall
    Dim objwriter As New System.IO.StreamWriter(Directory.GetCurrentDirectory + "\debug.txt")
    Dim colPositions As New System.Collections.Generic.List(Of Integer()) 'list of wall positions
    Dim intRoomHeight As Integer = 20
    Dim intRoomWidth As Integer = 15
    Dim colWalls As New System.Collections.Generic.List(Of Wall)
    Dim intWallsOffset As Integer = 1
    Dim intFloorOffset As Integer = 0
    Dim bolGameOver As Boolean = False
    Dim intCount As Integer = 0
    Dim intCount2 As Integer = 0
    Dim intCount3 As Integer = 0
    Dim colControlBuffer As New System.Collections.Generic.List(Of String)
    Dim Timer As Stopwatch
    Dim txtCommandBar As Texture2D
    Dim txtSignals As Texture2D
    Dim txtTitle As Texture2D
    Dim txtFinal As Texture2D
    Dim txtGameOver As Texture2D
    Dim strGameState As String = "Title"
    Dim bolActionLock As Boolean = False
    Public Sub New()
        graphics = New GraphicsDeviceManager(Me)

    End Sub

    Protected Overrides Sub Initialize()
        'TODO: Add your initialization logic here'
        MyBase.Initialize()
        'graphics.IsFullScreen = True
        'graphics.PreferredBackBufferHeight = 768
        'graphics.PreferredBackBufferWidth = 1366
        'graphics.ApplyChanges()
        graphics.PreferredBackBufferHeight = 456
        graphics.PreferredBackBufferWidth = 352
        graphics.ApplyChanges()
        'CurrentDungeon.setEnemies(colEnemies)
        'CurrentDungeon.setRooms(colRooms)
        'CurrentDungeon.setWalls(colWalls)
    End Sub
    Public Function GetSpriteSheet(ByVal GD As GraphicsDevice, ByVal strPath As String) As Texture2D
        Dim textureStream As System.IO.Stream = New System.IO.StreamReader(strPath).BaseStream
        Dim Sheet As Texture2D = Texture2D.FromStream(GD, textureStream)
        Return Sheet
    End Function
    Protected Overrides Sub LoadContent()
        'Dim i() As Integer



        ' open debug file
        'Dim strFileName As String = Directory.GetCurrentDirectory + "\debug.txt"

        'If System.IO.File.Exists(strFileName) = False Then
        '    System.IO.File.Create(strFileName)
        'End If
        'objwriter = New System.IO.StreamWriter(strFileName)
        Rover.SetSpriteFile(Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Rover1.png")
        MenuRover.SetSpriteFile(Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Rover1.png")
        txtCommandBar = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Command Bar.png")
        txtSignals = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Signals.png")
        txtTitle = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Title2.png")
        txtGameOver = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\GameOver.png")
        txtFinal = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Final Screen.png")

        ' TODO: use this.Content to load your game content here
        MyBase.LoadContent()
        ' Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = New SpriteBatch(GraphicsDevice)
        'Load the texture'
        'We are using Stream since i couldn't find how to make content in VB'
        'Dim textureStream As System.IO.Stream = New System.IO.StreamReader(MainCharacter.strSpriteFile).BaseStream
        'Loading the texture'
        Rover.SetTexture(GetSpriteSheet(GraphicsDevice, Rover.getSpriteFile))
        Rover.colFrontWalk.Add(New Rectangle(0, 0, 43, 43))
        Rover.colFrontWalk.Add(New Rectangle(43, 0, 43, 43))
        Rover.colFrontWalk.Add(New Rectangle(86, 0, 43, 43))
        Rover.colFrontWalk.Add(New Rectangle(129, 0, 43, 43))
        Rover.colFrontWalk.Add(New Rectangle(172, 0, 43, 43))
        Rover.colCurrentFrames = Rover.colFrontWalk
        Rover.SetHeight(43)
        Rover.SetWidth(43)
        Rover.SetPositionX(175)
        Rover.SetPositionY(400)

        MenuRover.SetTexture(GetSpriteSheet(GraphicsDevice, Rover.getSpriteFile))
        MenuRover.colFrontWalk.Add(New Rectangle(0, 0, 43, 43))
        MenuRover.colFrontWalk.Add(New Rectangle(43, 0, 43, 43))
        MenuRover.colFrontWalk.Add(New Rectangle(86, 0, 43, 43))
        MenuRover.colFrontWalk.Add(New Rectangle(129, 0, 43, 43))
        MenuRover.colFrontWalk.Add(New Rectangle(172, 0, 43, 43))
        MenuRover.colCurrentFrames = Rover.colFrontWalk
        MenuRover.SetHeight(43)
        MenuRover.SetWidth(43)
        MenuRover.SetPositionX(150)
        MenuRover.SetPositionY(350)

        'MainCharacter.vecPosition = New Vector2(100, 100)

        'textureStream = New System.IO.StreamReader(Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\BricksFull.png").BaseStream
        'BrickTemplate.Texture = Texture2D.FromStream(GraphicsDevice, textureStream)
        BrickTemplate.Texture = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Rubble.png")
        BrickTemplate.SetCollision(BrickTemplate.Texture.Width, BrickTemplate.Texture.Height, 22, 24, 0, 0)
        colPositions = GetPositions()
        For intX As Integer = -1 To intRoomHeight
            colPositions.Add(New Integer() {intRoomWidth, intX})
            colPositions.Add(New Integer() {0, intX})
        Next

        'For Y As Integer = -1 To intRoomHeight
        '    i = New Integer() {0, Y}
        '    colPositions.Add(i)
        'Next
        'For Y As Integer = -1 To intRoomHeight
        '    i = New Integer() {intRoomWidth, Y}
        '    colPositions.Add(i)
        'Next

        'i = New Integer() {3, 5}wsad
        'colPositions.Add(i)
        'i = New Integer() {4, 2}
        'colPositions.Add(i)
        ''i = New Integer() {8, -2}
        ''colPositions.Add(i)
        'i = New Integer() {13, 0}
        'colPositions.Add(i)
        'i = New Integer() {14, 0}
        'colPositions.Add(i)
        'i = New Integer() {13, -1}
        'colPositions.Add(i)
        'i = New Integer() {14, -1}
        'colPositions.Add(i)
        'i = New Integer() {8, -7}
        'colPositions.Add(i)
        'i = New Integer() {2, -9}
        'colPositions.Add(i)
        'i = New Integer() {10, -13}
        'colPositions.Add(i)
        'i = New Integer() {10, -14}
        'colPositions.Add(i)
        'i = New Integer() {11, -13}
        'colPositions.Add(i)
        'i = New Integer() {11, -14}
        'colPositions.Add(i)
        'i = New Integer() {1, -26}
        'colPositions.Add(i)
        'i = New Integer() {2, -27}
        'colPositions.Add(i)
        'i = New Integer() {3, -28}
        'colPositions.Add(i)
        'i = New Integer() {4, -29}
        'colPositions.Add(i)
        'i = New Integer() {5, -30}
        'colPositions.Add(i)
        'i = New Integer() {6, -31}
        'colPositions.Add(i)
        'i = New Integer() {7, -32}
        'colPositions.Add(i)
        'i = New Integer() {6, -33}
        'colPositions.Add(i)
        'i = New Integer() {5, -34}
        'colPositions.Add(i)
        'i = New Integer() {4, -35}
        'colPositions.Add(i)
        'i = New Integer() {3, -36}
        'colPositions.Add(i)
        'i = New Integer() {2, -37}
        'colPositions.Add(i)
        'i = New Integer() {1, -38}
        'colPositions.Add(i)
        'i = New Integer() {14, -30}
        'colPositions.Add(i)
        'i = New Integer() {13, -31}
        'colPositions.Add(i)
        'i = New Integer() {12, -32}
        'colPositions.Add(i)
        'i = New Integer() {11, -33}
        'colPositions.Add(i)
        'i = New Integer() {12, -34}
        'colPositions.Add(i)
        'i = New Integer() {13, -35}
        'colPositions.Add(i)
        'i = New Integer() {14, -36}
        'colPositions.Add(i)
        'i = New Integer() {1, -42}
        'colPositions.Add(i)
        'i = New Integer() {2, -43}
        'colPositions.Add(i)
        'i = New Integer() {3, -44}
        'colPositions.Add(i)
        'i = New Integer() {4, -45}
        'colPositions.Add(i)
        'i = New Integer() {5, -46}
        'colPositions.Add(i)
        'i = New Integer() {6, -47}
        'colPositions.Add(i)
        'i = New Integer() {7, -48}
        'colPositions.Add(i)
        'i = New Integer() {8, -49}
        'colPositions.Add(i)
        'i = New Integer() {9, -50}
        'colPositions.Add(i)
        'i = New Integer() {10, -51}
        'colPositions.Add(i)
        'i = New Integer() {11, -59}
        'colPositions.Add(i)
        'i = New Integer() {13, -63}
        'colPositions.Add(i)
        'i = New Integer() {8, -67}
        'colPositions.Add(i)
        'i = New Integer() {4, -62}
        'colPositions.Add(i)
        'i = New Integer() {5, -68}
        'colPositions.Add(i)
        'i = New Integer() {11, -75}
        'colPositions.Add(i)
        'i = New Integer() {3, -72}
        'colPositions.Add(i)
        'i = New Integer() {6, -78}
        'colPositions.Add(i)
        'i = New Integer() {1, -81}
        'colPositions.Add(i)
        'i = New Integer() {2, -81}
        'colPositions.Add(i)
        'i = New Integer() {3, -82}
        'colPositions.Add(i)
        'i = New Integer() {4, -82}
        'colPositions.Add(i)
        'i = New Integer() {5, -83}
        'colPositions.Add(i)
        'i = New Integer() {6, -83}
        'colPositions.Add(i)
        'i = New Integer() {7, -83}
        'colPositions.Add(i)
        'i = New Integer() {8, -83}
        'colPositions.Add(i)
        'i = New Integer() {8, -84}
        'colPositions.Add(i)
        'i = New Integer() {8, -85}
        'colPositions.Add(i)
        'i = New Integer() {8, -86}
        'colPositions.Add(i)
        'i = New Integer() {8, -87}
        'colPositions.Add(i)
        'i = New Integer() {8, -88}
        'colPositions.Add(i)
        'i = New Integer() {8, -89}
        'colPositions.Add(i)
        'i = New Integer() {8, -90}
        'colPositions.Add(i)
        'i = New Integer() {8, -91}
        'colPositions.Add(i)
        'i = New Integer() {8, -92}
        'colPositions.Add(i)
        'i = New Integer() {8, -93}
        'colPositions.Add(i)
        'i = New Integer() {8, -94}
        'colPositions.Add(i)
        'i = New Integer() {8, -95}
        'colPositions.Add(i)
        'i = New Integer() {8, -90}
        'colPositions.Add(i)
        'i = New Integer() {13, -83}
        'colPositions.Add(i)
        'i = New Integer() {14, -83}
        'colPositions.Add(i)
        'i = New Integer() {15, -83}
        'colPositions.Add(i)
        'i = New Integer() {15, -84}
        'colPositions.Add(i)
        'i = New Integer() {15, -85}
        'colPositions.Add(i)
        'i = New Integer() {15, -86}
        'colPositions.Add(i)
        'i = New Integer() {15, -87}
        'colPositions.Add(i)
        'i = New Integer() {15, -88}
        'colPositions.Add(i)
        'i = New Integer() {15, -89}
        'colPositions.Add(i)
        'i = New Integer() {15, -90}
        'colPositions.Add(i)
        'i = New Integer() {15, -91}
        'colPositions.Add(i)
        'i = New Integer() {15, -92}
        'colPositions.Add(i)
        'i = New Integer() {15, -93}
        'colPositions.Add(i)
        'i = New Integer() {15, -94}
        'colPositions.Add(i)
        'i = New Integer() {15, -95}
        'colPositions.Add(i)
        'i = New Integer() {15, -96}
        'colPositions.Add(i)
        'i = New Integer() {15, -97}
        'colPositions.Add(i)
        'i = New Integer() {15, -98}
        'colPositions.Add(i)
        'i = New Integer() {15, -99}
        'colPositions.Add(i)
        'i = New Integer() {15, -100}
        'colPositions.Add(i)
        'i = New Integer() {14, -100}
        'colPositions.Add(i)
        'i = New Integer() {13, -100}
        'colPositions.Add(i)
        'i = New Integer() {12, -100}
        'colPositions.Add(i)
        'i = New Integer() {11, -100}
        'colPositions.Add(i)
        'i = New Integer() {10, -100}
        'colPositions.Add(i)
        'i = New Integer() {9, -100}
        'colPositions.Add(i)
        'i = New Integer() {8, -100}
        'colPositions.Add(i)
        'i = New Integer() {7, -100}
        'colPositions.Add(i)
        'i = New Integer() {6, -100}
        'colPositions.Add(i)
        'i = New Integer() {5, -100}
        'colPositions.Add(i)
        'i = New Integer() {1, -96}
        'colPositions.Add(i)
        'i = New Integer() {2, -96}
        'colPositions.Add(i)
        'i = New Integer() {3, -96}
        'colPositions.Add(i)
        'i = New Integer() {4, -96}
        'colPositions.Add(i)
        'i = New Integer() {5, -96}
        'colPositions.Add(i)
        'i = New Integer() {6, -96}
        'colPositions.Add(i)
        'i = New Integer() {7, -96}
        'colPositions.Add(i)
        'i = New Integer() {8, -96}
        'colPositions.Add(i)
        'i = New Integer() {5, -101}
        'colPositions.Add(i)
        'i = New Integer() {5, -102}
        'colPositions.Add(i)
        'i = New Integer() {5, -103}
        'colPositions.Add(i)
        'i = New Integer() {5, -104}
        'colPositions.Add(i)
        'i = New Integer() {5, -105}
        'colPositions.Add(i)
        'For x As Integer = 11 To 14
        '    For y As Integer = -21 To -23 Step -1
        '        i = New Integer() {x, y}
        '        colPositions.Add(i)
        '    Next
        'Next

        For Each intPair() As Integer In colPositions
            colWalls.Add(New Wall(BrickTemplate.Texture, BrickTemplate.Texture.Width, BrickTemplate.Texture.Height, BrickTemplate.recCollision.Width * intPair(0), BrickTemplate.recCollision.Height * intPair(1), 22, 24, True, False, False))
            'colWalls.Add(New Wall(BrickTemplate.Texture, BrickTemplate.Texture.Width, BrickTemplate.Texture.Height, BrickTemplate.recCollision.Width * intPair(0), BrickTemplate.recCollision.Height * intPair(1), 22, 24))
        Next


        'textureStream = New System.IO.StreamReader(Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Floor.png").BaseStream
        'FloorTemplate.Texture = Texture2D.FromStream(GraphicsDevice, textureStream)
        FloorTemplate.Texture = GetSpriteSheet(GraphicsDevice, Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Surface.png")
        'FloorTemplate.SetCollision(48, 56, 48, 56, 0, 0)
        FloorTemplate.SetCollision(22, 24, 22, 24, 0, 0)
    End Sub

    Protected Overrides Sub UnloadContent()
        : MyBase.UnloadContent()
        'TODO: Unload any non ContentManager content here'
    End Sub
    Protected Overrides Sub Update(ByVal gameTime As Microsoft.Xna.Framework.GameTime)
        'Allows the game to exit'
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed Then
            Me.Exit()
        End If
        If Keyboard.GetState.IsKeyDown(Keys.Escape) Then
            Me.Exit()
        End If
        If strGameState = "Title" Then
            MenuRover.ChangeAnimationFrame(Rover.colCurrentFrames)
            If Keyboard.GetState.IsKeyDown(Keys.Enter) Or Keyboard.GetState.IsKeyDown(Keys.Space) Then
                strGameState = "Game"
                Timer = Stopwatch.StartNew
            End If
        ElseIf strGameState = "Game" Then

            If bolGameOver <> True Then
                intCount3 += 1
                intCount += 1
                If intCount = 1 Then
                    For Each x As Wall In colWalls
                        x.intPositionY += intWallsOffset
                        x.recCollision.Y += intWallsOffset
                    Next
                    intFloorOffset += 1
                    If intFloorOffset = 24 Then
                        intFloorOffset = 0
                    End If
                    intCount = 0
                    intCount2 += 1
                End If
                If intCount2 = 24 Then
                    colWalls.Add(New Wall(BrickTemplate.Texture, BrickTemplate.Texture.Width, BrickTemplate.Texture.Height, 0, -24, 22, 24, True, False, False))
                    colWalls.Add(New Wall(BrickTemplate.Texture, BrickTemplate.Texture.Width, BrickTemplate.Texture.Height, intRoomWidth * 22, -24, 22, 24, True, False, False))
                    intCount2 = 0
                End If
                If Keyboard.GetState.IsKeyDown(Keys.A) Or Keyboard.GetState.IsKeyDown(Keys.Left) Then
                    If Keyboard.GetState.IsKeyDown(Keys.W) Or Keyboard.GetState.IsKeyDown(Keys.Up) Then
                        colControlBuffer.Add("upleft")
                    ElseIf Keyboard.GetState.IsKeyDown(Keys.S) Or Keyboard.GetState.IsKeyDown(Keys.Down) Then
                        colControlBuffer.Add("downleft")
                    Else
                        colControlBuffer.Add("left")
                    End If
                ElseIf Keyboard.GetState.IsKeyDown(Keys.D) Or Keyboard.GetState.IsKeyDown(Keys.Right) Then
                    If Keyboard.GetState.IsKeyDown(Keys.W) Or Keyboard.GetState.IsKeyDown(Keys.Up) Then
                        colControlBuffer.Add("upright")
                    ElseIf Keyboard.GetState.IsKeyDown(Keys.S) Or Keyboard.GetState.IsKeyDown(Keys.Down) Then
                        colControlBuffer.Add("downright")
                    Else
                        colControlBuffer.Add("right")
                    End If
                ElseIf Keyboard.GetState.IsKeyDown(Keys.W) Or Keyboard.GetState.IsKeyDown(Keys.Up) Then
                    colControlBuffer.Add("up")
                ElseIf Keyboard.GetState.IsKeyDown(Keys.S) Or Keyboard.GetState.IsKeyDown(Keys.Down) Then
                    colControlBuffer.Add("down")
                Else
                    colControlBuffer.Add("")
                End If
                If Timer.ElapsedMilliseconds > 0 Then
                    If colControlBuffer(0) = "left" Or colControlBuffer(0) = "downleft" Or colControlBuffer(0) = "upleft" Then
                        Rover.vecPosition.X -= 2S
                        Rover.SetPositionX(Convert.ToInt32(Rover.vecPosition.X))
                    End If
                    If colControlBuffer(0) = "right" Or colControlBuffer(0) = "downright" Or colControlBuffer(0) = "upright" Then
                        Rover.vecPosition.X += 2
                        Rover.SetPositionX(Convert.ToInt32(Rover.vecPosition.X))
                    End If
                    If colControlBuffer(0) = "up" Or colControlBuffer(0) = "upleft" Or colControlBuffer(0) = "upright" Then
                        If Rover.vecPosition.Y <> 0 Then
                            Rover.vecPosition.Y -= 2
                            Rover.SetPositionY(Convert.ToInt32(Rover.vecPosition.Y))
                        End If
                    End If
                    If colControlBuffer(0) = "down" Or colControlBuffer(0) = "downleft" Or colControlBuffer(0) = "downright" Then
                        If Rover.vecPosition.Y + Rover.intHeight <> 456 Then
                            Rover.vecPosition.Y += 1
                            Rover.SetPositionY(Convert.ToInt32(Rover.vecPosition.Y))
                        End If
                    End If
                    colControlBuffer.RemoveAt(0)
                End If
                If detectCollisions(Rover.recCollision, colWalls) Then
                    bolGameOver = True
                    Rover.intInvincibleTime = 30
                End If
            End If
            If bolGameOver = True And Rover.intInvincibleTime = 0 Then
                strGameState = "GameOver"
            End If
            If intCount3 = 6000 Then
                strGameState = "Final"
            End If
            Rover.ChangeAnimationFrame(Rover.colCurrentFrames)
            Rover.intInvincibleTime -= 1
            'TODO: Add your update logic here'
            ElseIf strGameState = "GameOver" Then
                If Keyboard.GetState.IsKeyDown(Keys.Enter) = True Or Keyboard.GetState.IsKeyDown(Keys.Space) = True Then
                    'strGameState = "Title"
                    Me.Exit()
                End If
            ElseIf strGameState = "Final" Then
                If Keyboard.GetState.IsKeyDown(Keys.Enter) = True Or Keyboard.GetState.IsKeyDown(Keys.Space) = True Then
                    'strGameState = "Title"
                    Me.Exit()
                End If
            End If
            MyBase.Update(gameTime)
    End Sub
    Protected Overrides Sub Draw(ByVal gameTime As Microsoft.Xna.Framework.GameTime)
        Dim intX As Integer = 14
        GraphicsDevice.Clear(Color.CornflowerBlue)
        'TODO: Add your drawing code here'
        'Draw the sprite'

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend)
        If strGameState = "Title" Then
            spriteBatch.Draw(txtTitle, New Rectangle(0, 0, 352, 456), Color.White)
            MenuRover.Draw(spriteBatch)
        ElseIf strGameState = "Game" Then
            For x As Integer = 0 To intRoomWidth
                For y As Integer = -1 To intRoomHeight
                    spriteBatch.Draw(FloorTemplate.Texture, New Rectangle(x * FloorTemplate.recCollision.Width, (y * FloorTemplate.recCollision.Height) + intFloorOffset, FloorTemplate.recCollision.Width, FloorTemplate.recCollision.Height), Color.White)
                Next
            Next
            If Rover.intInvincibleTime Mod 10 < 5 Then
                Rover.Draw(spriteBatch)
            End If

            For Each Brick As Wall In colWalls
                If Brick.GetDraw = True Then
                    spriteBatch.Draw(Brick.Texture, New Rectangle(Brick.intPositionX, Brick.intPositionY, Brick.intTextureWidth, Brick.intTextureHeight), Color.White)
                End If
            Next
            spriteBatch.Draw(txtCommandBar, New Rectangle(10, 10, 24, 160), Color.White)
            For Each Signal As String In colControlBuffer
                If Signal = "left" Or Signal = "upleft" Or Signal = "downleft" Then
                    spriteBatch.Draw(txtSignals, New Rectangle(14, intX, 16, 2), New Rectangle(0, 0, 16, 2), Color.White)
                End If
                If Signal = "down" Or Signal = "downright" Or Signal = "downleft" Then
                    spriteBatch.Draw(txtSignals, New Rectangle(14, intX, 16, 2), New Rectangle(0, 2, 16, 2), Color.White)
                End If
                If Signal = "up" Or Signal = "upleft" Or Signal = "upright" Then
                    spriteBatch.Draw(txtSignals, New Rectangle(14, intX, 16, 2), New Rectangle(0, 4, 16, 2), Color.White)
                End If
                If Signal = "right" Or Signal = "upright" Or Signal = "downright" Then
                    spriteBatch.Draw(txtSignals, New Rectangle(14, intX, 16, 2), New Rectangle(0, 6, 16, 2), Color.White)
                End If
                intX += 2
            Next
        ElseIf strGameState = "GameOver" Then
            spriteBatch.Draw(txtGameOver, New Rectangle(0, 0, 352, 456), Color.White)
        ElseIf strGameState = "Final" Then
            spriteBatch.Draw(txtFinal, New Rectangle(0, 0, 352, 456), Color.White)
        End If
        spriteBatch.End()
        MyBase.Draw(gameTime)
    End Sub
    Public Function detectCollisions(ByVal recCollisionTest As Rectangle, ByRef colWalls As System.Collections.Generic.List(Of Wall)) As Boolean
        For Each wall As Wall In colWalls
            If Not (recCollisionTest.Bottom <= wall.recCollision.Top Or recCollisionTest.Top >= wall.recCollision.Bottom Or recCollisionTest.Left >= wall.recCollision.Right Or recCollisionTest.Right <= wall.recCollision.Left) Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function GetPositions() As System.Collections.Generic.List(Of Integer())
        Dim ColPositions As New System.Collections.Generic.List(Of Integer())
        Dim strFileName As String = Convert.ToString(Directory.GetCurrentDirectory) + "\Game Assets\Level file.txt"
        Dim objReader As New System.IO.StreamReader(strFileName)
        Dim strCurrentLine As String
        Dim strSplitLine() As String
        Dim intLine As Integer = 1

        Do While objReader.Peek() <> -1
            intLine += 1
            strCurrentLine = objReader.ReadLine()
            strSplitLine = Split(strCurrentLine, Convert.ToChar(Keys.Tab))
            For i As Integer = 0 To strSplitLine.Length - 1
                If strSplitLine(i) = "x" Then
                    ColPositions.Add(New Integer() {i + 1, intLine * -1})
                End If
            Next
        Loop
        
        Return ColPositions
    End Function
End Class
Module mdlMain
    Sub Main()
        : Using game As New Game
            : game.Run()
            : End Using
    End Sub
End Module
Public Class Wall
    Public Texture As Texture2D
    Public intTextureHeight As Integer
    Public intTextureWidth As Integer
    Public recCollision As Rectangle
    Public intPositionX As Integer
    Public intPositionY As Integer
    Private bolDraw As Boolean = False
    Public Sub New()

    End Sub
    Public Sub New(ByVal txTexture As Texture2D, ByVal intPixelsWide As Integer, ByVal intpixelsTall As Integer)
        Texture = txTexture
        intTextureHeight = intpixelsTall
        intTextureWidth = intPixelsWide
    End Sub
    Public Sub New(ByVal txTexture As Texture2D, ByVal intPixelsWide As Integer, ByVal intpixelsTall As Integer, ByVal intCollisionX As Integer, ByVal intcollisionY As Integer, ByVal intCollisionWide As Integer, ByVal intcollisionTall As Integer)
        Texture = txTexture
        intTextureHeight = intpixelsTall
        intTextureWidth = intPixelsWide
        recCollision.X = intCollisionX
        recCollision.Y = intcollisionY
        recCollision.Width = intCollisionWide
        recCollision.Height = intcollisionTall
        intPositionX = recCollision.X + recCollision.Width - Texture.Width
        intPositionY = recCollision.Y + recCollision.Height - Texture.Height
    End Sub
    Public Sub New(ByVal txTexture As Texture2D, ByVal intPixelsWide As Integer, ByVal intpixelsTall As Integer, ByVal intCollisionX As Integer, ByVal intcollisionY As Integer, ByVal intCollisionWide As Integer, ByVal intcollisionTall As Integer, ByVal Draw As Boolean, ByVal PlayerPassThrough As Boolean, ByVal EnemyPassThrough As Boolean)
        Texture = txTexture
        intTextureHeight = intpixelsTall
        intTextureWidth = intPixelsWide
        recCollision.X = intCollisionX
        recCollision.Y = intcollisionY
        recCollision.Width = intCollisionWide
        recCollision.Height = intcollisionTall
        intPositionX = recCollision.X + recCollision.Width - Texture.Width
        intPositionY = recCollision.Y + recCollision.Height - Texture.Height
        bolDraw = Draw
    End Sub
    Public Sub SetCollision(ByVal intTextureWide As Integer, ByVal intTextureTall As Integer, ByVal intCollisionWide As Integer, ByVal intCollisionTall As Integer, ByVal intCollisionX As Integer, ByVal intCollisionY As Integer)
        recCollision.X = intCollisionX
        recCollision.Y = intCollisionY
        recCollision.Width = intCollisionWide
        recCollision.Height = intCollisionTall
        intTextureWide = intTextureWide
        intTextureHeight = intTextureTall
        intPositionX = recCollision.X + recCollision.Width - Texture.Width
        intPositionX = recCollision.Y + recCollision.Height - Texture.Height
    End Sub
    Public Sub SetDraw(ByVal bolValue As Boolean)
        bolDraw = bolValue
    End Sub
    Public Function GetDraw() As Boolean
        Return bolDraw
    End Function
End Class
Public Class AnimationFrame
    Public recFrame As New Rectangle
    Public intAnimationTime As Integer
    Public intOffsetX As Integer 'the two offset integers are for effects that move with the character
    Public intOffsetY As Integer
    Public Sub New(ByVal Frame As Rectangle, Optional ByVal Time As Integer = 10, Optional ByVal OffsetX As Integer = 0, Optional ByVal OffsetY As Integer = 0)
        recFrame = Frame
        intAnimationTime = Time
        intOffsetX = OffsetX
        intOffsetY = OffsetY
    End Sub
End Class

Public Class character
    Private spriteSheet As Texture2D
    Private strSpriteFile As String
    Public colFrontWalk As New System.Collections.Generic.List(Of Rectangle)
    Public colCurrentFrames As System.Collections.Generic.List(Of Rectangle)
    Public intCurrentFrame As Integer
    Public intFrames As Integer 'used for counting frames between animation, increments everytime the same frame is displayed
    Public intFrameMax As Integer = 10 'the number of frames until the animation switches
    Public dblSpeed As Double = 2
    Private timer As Stopwatch
    Public vecPosition As Vector2
    Public intHeight As Integer
    Public intWidth As Integer
    Public recCollision As Rectangle
    Public ColTextures As New System.Collections.Generic.Dictionary(Of String, Texture2D)
    Public intInvincibleTime As Integer = 0
    Private bolWalkRight As Boolean = False
    Private bolwalkleft As Boolean = False

    Public Sub SetHeight(ByVal Height As Integer)
        intHeight = Height
        recCollision.Height = Height
    End Sub
    Public Sub SetWidth(ByVal Width As Integer)
        intWidth = Width
        recCollision.Width = Width
    End Sub
    Public Sub SetTexture(ByRef texture As Texture2D)
        spriteSheet = texture
    End Sub
    Public Sub SetSpriteFile(ByRef File As String)
        strSpriteFile = File
    End Sub
    Public Function getSpriteFile() As String
        Return strSpriteFile
    End Function

    Public Sub SetPositionX(ByVal intXAddition As Integer)
        vecPosition.X = intXAddition
        recCollision.X = intXAddition
    End Sub
    Public Sub SetPositionY(ByVal intYAddition As Integer)
        vecPosition.Y = intYAddition
        recCollision.Y = intYAddition
    End Sub
    Public Sub ChangeAnimationFrame(ByVal colFrames As System.Collections.Generic.List(Of Rectangle))

        If intFrames < intFrameMax Then
            intFrames += 1
        Else
            intFrames = 0
            If intCurrentFrame + 1 >= colFrames.Count Then
                intCurrentFrame = 0
            Else
                intCurrentFrame += 1
            End If
        End If

    End Sub
    
    Public Sub Draw(ByRef SpriteBatch As SpriteBatch)
        SpriteBatch.Draw(spriteSheet, vecPosition, colCurrentFrames.Item(intCurrentFrame), Color.White)
    End Sub
    
End Class




