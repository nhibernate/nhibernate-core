<?xml version="1.0"?>
<!--
	Customization of the layout of NHibernate Documentation
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="docbook-xsl/html/chunk.xsl"/>
	<xsl:include href="common.xsl"/>


	<!--
		Output in ASCII because Hibernate.org web server is misconfigured
		(has AddDefaultCharset UTF-8), and xsltproc doesn't replace &#160; with &nbsp;
	-->
	<xsl:param name="chunker.output.encoding" select="'ASCII'"/>
	<xsl:output method="html"
				encoding="ASCII"
				indent="no"/>

</xsl:stylesheet>
