<%@ Application Inherits="Website.Global" Language="C#" %>
<script Language="C#" runat="server">

  protected void Application_Start(Object sender, EventArgs e) 
  {
      Start();
  }
  
  protected void Application_BeginRequest(Object sender, EventArgs e)
  {
      BeginRequest();
  }

</script>
