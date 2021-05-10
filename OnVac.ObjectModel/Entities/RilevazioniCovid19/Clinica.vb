Namespace Entities
    <Serializable()>
    Public Class Clinica
        Public Property Tumore As Boolean
        Public Property Diabete As Boolean
        Public Property MalattiaCardiovascolare As Boolean
        Public Property ImmunoDeficienza As Boolean
        Public Property MalattiaRespiratoria As Boolean
        Public Property MalattiaRenale As Boolean
        Public Property MalattiaMetabolica As Boolean
        Public Property Altro As Boolean
        Public Property Note As String
        Public Property Obesita_BMI_30_40 As Boolean
        Public Property Obesita_BMI_Maggiore_40 As Boolean
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
    End Class

End Namespace

