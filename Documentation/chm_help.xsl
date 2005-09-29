<?xml version="1.0"?>
<!--
	Customization of the layout of NHibernate Documentation
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="docbook-xsl/htmlhelp/htmlhelp.xsl"/>

	<xsl:include href="common.xsl"/>


	<!-- Customized HTML stylesheet !-->
	<xsl:param name="html.stylesheet" select="'style.css'" />

	<!-- TODO: Check this !-->

	<xsl:param name="suppress.navigation" select="0"/>
	<xsl:param name="htmlhelp.hhc.binary" select="0"/>
	<xsl:param name="htmlhelp.hhc.folders.instead.books" select="0"/>

	<xsl:param name="generate.index" select="1" />

</xsl:stylesheet>
