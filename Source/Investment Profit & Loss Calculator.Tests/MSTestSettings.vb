#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

' Tests change the thread culture and create Windows Forms windows,
' so they must not run in parallel.
<Assembly: DoNotParallelize>
