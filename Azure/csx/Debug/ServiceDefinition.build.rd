<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Azure" generation="1" functional="0" release="0" Id="6c14e943-85ca-4efc-a215-726eab670572" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebSite:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Azure/AzureGroup/LB:WebSite:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="WebSite:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Azure/AzureGroup/MapWebSite:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebSiteInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Azure/AzureGroup/MapWebSiteInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebSite:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Azure/AzureGroup/WebSite/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapWebSite:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Azure/AzureGroup/WebSite/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebSiteInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Azure/AzureGroup/WebSiteInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WebSite" generation="1" functional="0" release="0" software="C:\Users\Ameen\Documents\Stockwinners\Azure\csx\Debug\roles\WebSite" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebSite&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebSite&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Azure/AzureGroup/WebSiteInstances" />
            <sCSPolicyFaultDomainMoniker name="/Azure/AzureGroup/WebSiteFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="WebSiteFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WebSiteInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="39d16474-b739-4650-ad83-144f3b784f9d" ref="Microsoft.RedDog.Contract\ServiceContract\AzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="2ada2634-df36-41d4-b91e-0e7ba273ed4c" ref="Microsoft.RedDog.Contract\Interface\WebSite:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Azure/AzureGroup/WebSite:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>