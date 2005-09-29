<?xml version="1.0"?>
<!--
	Customization of the layout of NHibernate Documentation
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="docbook-xsl/html/docbook.xsl"/>

	<xsl:include href="common.xsl"/>


	<!-- Customized HTML stylesheet !-->
	<xsl:param name="html.stylesheet" select="'NHibernate.Documentation_files/style.css'" />

	<!-- Allow referring to images without writing the path !-->
	<xsl:param name="img.src.path">NHibernate.Documentation_files/</xsl:param>

</xsl:stylesheet>
