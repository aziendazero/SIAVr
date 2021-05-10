<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GestioneAlias.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneAlias" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>GestioneAlias</title>
		<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
		<script type="text/javascript" language="javascript">
			location.replace('../HPazienti/GestionePazienti/ricercaPazienteBis.aspx?alias=true&menu_dis=<%= OnVacContext.MenuDis%>');
		</script>
	</head>
	<body>	
	</body>
</html>
