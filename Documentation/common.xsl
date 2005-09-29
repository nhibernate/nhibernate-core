<?xml version="1.0"?>
<!--
	Customization of the layout of NHibernate Documentation
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<!-- Avoid numbered filenames (use "id") !-->
	<xsl:param name="use.id.as.filename" select="'1'"></xsl:param>

	<!-- Don't divide chapters in many files !-->
    <xsl:param name="chunk.section.depth" select="0"></xsl:param>

	<!-- Don't use graphics, use a simple number style !-->
	<xsl:param name="callout.graphics" select="'0'"></xsl:param>

</xsl:stylesheet>
