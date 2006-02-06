<?xml version="1.0"?>
<!--
	Customization of the layout of NHibernate Documentation
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="../support/docbook-xsl/htmlhelp/htmlhelp.xsl"/>

<!--###################################################
					 HTML Settings
	################################################### -->   

	<xsl:param name="html.stylesheet">shared/css/html.css</xsl:param>

	<!-- These extensions are required for table printing and other stuff -->
	<xsl:param name="use.extensions">1</xsl:param>
	<xsl:param name="tablecolumns.extension">0</xsl:param>
	<xsl:param name="callout.extensions">1</xsl:param>
	<xsl:param name="graphicsize.extension">0</xsl:param>

	<xsl:param name="preferred.mediaobject.role">chm</xsl:param>

<!--###################################################
					  Table Of Contents
	################################################### -->   

	<!-- Generate the TOCs for named components only -->
	<xsl:param name="generate.toc">
		book   toc
	</xsl:param>
	
	<!-- Show only Sections up to level 3 in the TOCs -->
	<xsl:param name="toc.section.depth">3</xsl:param>
	
<!--###################################################
						 Labels
	################################################### -->   

	<!-- Label Chapters and Sections (numbering) -->
	<xsl:param name="chapter.autolabel">1</xsl:param>
	<xsl:param name="section.autolabel" select="1"/>
	<xsl:param name="section.label.includes.component.label" select="1"/>

<!--###################################################
						 Callouts
	################################################### -->

	<!-- Don't use graphics, use a simple number style -->
	<xsl:param name="callout.graphics">0</xsl:param>

	<!-- Place callout marks at this column in annotated areas -->
	<xsl:param name="callout.defaultcolumn">90</xsl:param>

<!--###################################################
						  Misc
	################################################### -->   

	<!-- Placement of titles -->
	<xsl:param name="formal.title.placement">
		figure after
		example before
		equation before
		table before
		procedure before
	</xsl:param>    

	<xsl:param name="suppress.navigation" select="0"/>
	<xsl:param name="htmlhelp.hhc.binary" select="0"/>
	<xsl:param name="htmlhelp.hhc.folders.instead.books" select="0"/>

	<xsl:param name="generate.index" select="1" />

</xsl:stylesheet>
