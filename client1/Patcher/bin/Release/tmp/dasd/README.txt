Copyright (c) 1999-2017 Xerox Corporation. All Rights Reserved.

=======================================================================
FreeFlow(R) VI Design Pro (VDP) 15.0 Service Pack 1 Release
=======================================================================

This README file contains Release Notes for FreeFlow VI Design Pro
15.0 Service Pack 1 Release.

Contents:
---------

- Introduction
- VIPP(R) Customer Forum
- Contents of VI Design Pro 15.0 Service Packs
- FreeFlow(R) VI Design Pro 15.0 Release Notes
- FreeFlow(R) VI Compose 15.0b Release Notes
- Contents of Previous Release Versions


**********************
**** Introduction ****
**********************

Our software product names have changed since the FreeFlow VI Suite 10.0 Release.
The new product names are as follows:

LEGACY Product Name                             NEW Product Name
--------------------------------------------    --------------------------------
FreeFlow VI Interpreter                         FreeFlow VI Compose
FreeFlow VI Interpreter Open Edition            FreeFlow VI Compose Open Edition
FreeFlow VI Designer                            FreeFlow VI Design Pro
FreeFlow VI PDF Originator                      FreeFlow VI eCompose
FreeFlow VIPP Pro Publisher                     FreeFlow VI Design Express

All other products not mentioned in this list keep the same name used
in the previous FreeFlow VI Suite Release.

References to the VIPP language, commands and variable information format
remain unchanged.

The full FreeFlow VI Suite product documentation and installation instructions
are found on a separate CD:

     FreeFlow VI Suite 15.0 Documentation

The PDF version of the FreeFlow VI Compose 15.0 Reference Manual is the complete reference manual - the online version installed with FreeFlow VI Design Pro is a subset consisting of only "The VIPP Language" and "Error Messages" sections.  In addition, the PDF versions of the manuals include illustrations and screen shots not available in the online help.


********************************
**** VIPP(R) Customer Forum ****
********************************

For questions and information regarding FreeFlow VI Design Pro, FreeFlow VI Compose,
or any other FreeFlow VI Suite software products log onto the VIPP Customer Forum at
"http://vippsupport.xerox.com" website.


******************************************************
**** FreeFlow(R) VI Design Pro 15.0 Service Packs ****
******************************************************

-------------------------------------
** Contents of 15.0 Service Pack 1 **
-------------------------------------
- Upgrade to FreeFlow VI Compose (VIC) 15.0b release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Upgrade to OpenSSL 1.0.2j version.

It also includes all the previous 15.0 release contents.


******************************************************
**** FreeFlow(R) VI Design Pro 15.0 Release Notes ****
******************************************************

New Features
==================
- Upgrade to FreeFlow VI Compose (VIC) 15.0 release.
  See 'X:\vide\xgf\README.txt' file for release details.

- SmartEditor dialog for DRAWPFF command to support PDF Fillable Form Fields.

Enhancements
==================
- Font resources listed in the "fonts" sub-tab of the Resources tab panel can
  be selected to display the full character set table. This can generate more
  than one page onscreen showing all character glyphs available based on the
  character encoding specified for the font.

- Added Transparency and Tint support for color option in SETTXC and INDEXCOLOR 
  SmartEditor dialogs.

- Enabled SmartEditor dialog for SETFTSP (font spacing). It uses same SmartEditor
  GUI panel as the SETFONT command.  

- Added colorspaces (/SCS_CMYKB, /SCS_CMYKG, /SCS_CMYKO, /SCS_CMYKV, /SCS_SV, /SCS_V), 
  fifth color Orange, Spot Color and Clear Dry Ink option in SETCOL SmartEditor GUI
  panel.   

- Upgrade to Adobe PDFL SDK 15.0.1 version.


Bug Fixes
==================
- Fixed AR#VIPP-29: SHROW cmd empty after Error Parameter pop-up msg

- Fixed AR#VIPP-86: vippide.exe has stopped working when SMART-Edit SETPARAMS

- Fixed AR#VIPP-87: PS Error: syntax error in )

- Fixed AR#VIPP-102: The 'Text Data' string '<01 40 f3 a4 dd e2 b1> CS' is invalid.

- Fixed AR#VIPP-108: vippide.exe has stopped working.

- Fixed AR#VIPP-139: Hot spot move in VDP.

- Fixed AR#VIPP-26: Using Large fonts text truncation for /Width and /CellStroke parameter
  with SHROW and BEGINTABLE SE in JA build.

- Fixed AR#VIPP-24: Text overlapping with Windows Large Fonts for SHROW, BEGINTABLE, 
  PDFBOUND Smart Editors.

***************************************
**** Software License Requirements ****
***************************************

To fully enable the FreeFlow VI Design Pro software you must obtain the proper license file from your Xerox representative. You will need to supply both the system Host ID and the product Version numbers displayed when executing the 'GetHostID.exe' utility available on the installation CD.


*********************************************
**** System Requirements and Performance ****
*********************************************

The FreeFlow VI Design Pro software provides a WYSIWYG (What You See Is What You Get) environment for the development of VIPP applications.  In order to assure that what is displayed in the VI Design Pro corresponds with the output produced on a VIPP-enabled PostScript printer, VI Design Pro utilizes both the FreeFlow VI Compose runtime and an actual PostScript Interpreter/RIP (Raster Image Processor) when processing and rendering your VIPP applications.  In addition, VI Design Pro provides some features not normally available on a printer - these include random access to job pages (browsing) as well as adjustment of the display's zoom and orientation.

The VI Design Pro, the VI Compose runtime, and the PostScript Interpreter/RIP require more processing power and memory than standard desktop applications to give satisfactory performance.  

The VI Design Pro can be installed on Windows Server 2008 (R1/R2), Windows 7 SP1, Windows 8, Windows 8.1, Windows Server 2012 (R1/R2) and Windows 10. Both 32-bit and 64-bit Windows OS platforms are supported. Minimum disk space required is 350 MBytes for software installation.

The MINIMUM hardware requirements for VI Design Pro are:

2.0 GHz Pentium 4
4.0 GB RAM (2GB of free physical memory available to the application itself)

Running VI Design Pro on less powerful hardware or with less RAM is not recommended.  If you are stuck with a slower processor _and_ less RAM (as might be the case for some older laptops), excessive paging (swapping to/from virtual memory) may combine with the slower processor speed to severely impair performance. In these situations, bumping up the RAM to at least 3.0 GB may help somewhat. 

In addition, the following Windows PC desktop settings are recommended:

1024x768 screen resolution (minimum), higher resolution preferred.
256 colors (minimum), 65535 colors (16-bit color) preferred.


**********************************
**** Job Data and Performance ****
**********************************

As mentioned above, VI Design Pro incorporates both the VI Compose runtime and a PostScript Interpreter/RIP.  In addition, however, the VI Design Pro presents a GUI (Graphical User Interface) running on a Windows-based workstation, which places certain restrictions on the amount of job data that can be handled within the VI Design Pro.  Attempting to load production volumes of job data into the VI Design Pro will certainly cause problems.  In this regard, you should think of VI Design Pro as a Windows application (which it is) rather than as a production printer (which it is not).  You wouldn't try to load a 500 MB line mode file into WordPad (not without making a good set of backups first 8-), so you shouldn't try to do this with VI Design Pro, either.

Put another way, the VI Design Pro is NOT intended to ingest large amounts of job data - VI Design Pro is a design tool which is intended to be used during application development.  Use just enough line mode or database mode data to verify that your application looks the way you think it should look, and behaves the way you think it should behave - typically a few pages to (max) a couple of dozen pages worth.  The performance of the VI Design Pro during both loading and browsing will suffer in direct proportion to the amount of job data ingested.  


*********************************************************
**** Reconciliation and Graphic Element Restrictions ****
*********************************************************

Immediately after invoking the VI Compose and the PostScript Interpreter/RIP to render a given page of your VIPP application, the VI Design Pro taps into the VI Compose to get information about the graphic elements that were actually rendered to the graphic display.  Only those graphic elements rendered through the use of VIPP marking commands can be accessed for graphic manipulation by VI Design Pro - those rendered through use of 'raw' PostScript are not accessible.  The VI Design Pro then performs an analysis of the current state of the VIPP source code in your application and references this against the graphic elements the VI Compose has rendered to the current page.  If a particular rendered element on the graphic display can be matched with its corresponding fragment of VIPP source code, it is said to have been 'reconciled' with its source element.

Only 'reconciled' graphic elements may be grabbed, dragged, right-clicked, etc. - this is because to maintain the integrity of your VIPP application, any action performed within the graphic display must eventually be realized through manipulation of the corresponding VIPP source code fragment.  If a graphic element cannot be selected with the mouse, then it has not been reconciled.

Because PostScript (and therefore VIPP) code can be arbitrarily complex, there are limits to how effective the analysis portion of this reconciliation activity can be performed.  Current limitations in this area include (but are not restricted to) reconciliation of graphic elements rendered via use of conditional execution, loops, or variable substitution.  Use of these constructs in your VIPP application source can confuse the reconciliation process, resulting in either a failure to reconcile or a reconcile mismatch - the latter case being one where a 'View Source' for a particular element on the graphic display points to an incorrect location in the VIPP source.  While we are continuously improving the analysis capabilities of VI Design Pro, these limitations mean that reconciliation is performed as a "best effort" activity.


************************
**** Proof-Printing ****
************************

You can only use this option with a PostScript printer.  If the printer already has the VI Compose installed, then the version of VIPP resident on the printer MUST be of the same version or higher than the version of the VI Compose shipped with VI Design Pro.  To determine the version of the VI Compose shipped with VI Design Pro, choose 'Help -> About FreeFlow VI Design Pro' from the VI Design Pro main window pulldown menu.

The page range selection in ProofPrint refers to logical pages, while the graphic display shows the current and total physical pages of the application. This means that for multi-up applications the ProofPrint page range selection values may not correspond 1-to-1 with the actual number or position of logical pages. The user should select the range of pages to be proof printed based on the desired logical page numbers, not the physical page numbers. 


*********************
**** SmartEditor ****
*********************

In general, the SmartEditor cannot be relied on to automatically fix errors in VIPP command arguments or syntax that may exist in your VIPP source code.  This because the number of different ways a given VIPP command can be improperly formed is potentially infinite - beyond a certain point, the SmartEditor can no longer determine the intent of the code well enough to decide how to present editing options.  If upon invocation the SmartEditor detects command parameter problems (either wrong parameter/type or missing parameters) it cannot handle, it will notify the user of the failure to successfully parse the command.  In this circumstance, the user should select the VIPP command with the mouse and use the F1/Help feature to determine the nature of the problem and correct the syntax manually.


************************
**** Crash Recovery ****
************************

Upon restart after a crash, VI Design Pro will present an option to re-instantiate resources from what was in the resource execution cache when the application terminated abruptly or crashed.  The resource execution cache contains activities performed via the SmartEditor and/or WYSIWYG manipulation of graphical elements on the right-hand-side window, as well as manual resource edits as long as the manual edits were sent to the VI Compose via an 'F5' keypress or toolbar button execution of the resource. In other words, edits that are executed by the VI Compose (propagated to the right-hand-side graphical display) are saved in the resource execution cache. Upon restart of VI Design Pro after a crash, recovery is attempted (if the user so chooses) based on the cache contents.

Note that this crash recovery mechanism does NOT automatically overwrite the original job resources - this is by design.  After the crash recovery operation completes it is important to then save any desired changes that were recovered, just as if you had made a change without an explicit save during a normal editing session.  The usual system prompts to save the changes are presented in this situation.


****************************************
**** Text Wrapping Around an Object ****
****************************************

Due to differences in wrapping algorithms between Adobe InDesign and VIPP, a document using the new "Text Wrapping Around an Object" feature may display differently in Adobe InDesign than the VIPP job generated from such a document when displayed or printed. The rendering of the VIPP job should be validated by viewing the VI Project Container (VPC) in FreeFlow VI Explorer or FreeFlow VI Design Express Proof application to ensure that the "Text Wrapping" meets the user's expectations.


***********************
**** Miscellaneous ****
***********************

- SHPIT (text distortion)
When a job using the SHPIT command is loaded in VI Design Pro or VI Explorer, an unexpected error may occur from time to time. When this occurs simply acknowledge the error message and click on the "Refresh" button in VI Design Pro, or "Resubmit" button in VI Explorer.

- Percent signs in job data (such as using the '%' as a field delimiter in a .dbm file) may confuse the syntax highlighting feature.  This has no adverse effect on the VI Design Pro operation.

- Items in the Graphic Element Display cannot be selected on back pages if multi-up and tumble duplex are used at the same time.  Workaround: selection can be performed on back pages by temporarily commenting out TUMBLEDUPLEX_on or TWOUP/SETMULTIUP commands; selection can still be performed on front pages.

- Accelerating some resources (notably forms) may result in a PS error.  This is usually due to the use of the following syntax in the VIPP application:

{(MYFORM.PS) RUN} SETFORM 

This syntax is obsolete.  It is still supported for backward compatibility for use on VIPP-enabled printers, but it does not support resource acceleration in the VI Design Pro.  If you intend to accelerate the resource, use the following syntax instead:

(MYFORM.PS) CACHE SETFORM 

- Some elements may shift their placement when accelerated, particularly segments.  This is typically due to the absence of the CACHE command in the VIPP source, and is not a VI Design Pro software problem.  Remember, use of the CACHE command is mandatory when you want to accelerate a resource.

- When accelerating a resource, if the acceleration process is cancelled the project history log still indicates that an acceleration was performed. 

- When moving segment elements in the Graphic Element display, elements seem to disappear if they are dragged outside of the bounding box.  This is the expected behavior.  The bounding box (defined by the %%BoundingBox comment and highlighted by the dotted lines) is the area in which you can place elements of the segment.  Anything placed outside of this area will not be imaged.  If you want to enlarge this area you need to modify the %%BoundingBox statement (coordinates in this statement are always in points, see PLRM/2nd ed. [Red Book] p. 641).

- Frames defined via 'SETLKF' can now be selected, moved and resized.  Since a frame overlies the elements within it, to make it easier to select the frame itself the hotspot for the frame has been enlarged slightly.  Left-clicking just outside the outermost of the elements within the frame should highlight the frame, indicating that the frame itself can then be moved/resized.  To visibly differentiate a selected frame from other elements, the frame when selected will be highlighted in green.

- Graphical Element display support for VIPP fragments within VIPP frames (defined via the VIPP 'SETLKF' command) is limited to selection, 'view source' and 'properties'.  Dragging/resize of VIPP elements within frames (where detectable by the VI Design Pro) is disallowed in the current version of VI Design Pro.  In these circumstances, VIPP elements within frames will show a red outline when selected, indicating that they cannot be moved/resized.  In some cases (as with the use of the NEWFRAME command) the VI Design Pro cannot determine whether an element is within a frame or not and may not disable movement/resize for that element.  If such an element is moved/resized, the result can be unpredictable.  We hope to better address support for intra-frame elements in a future version of VI Design Pro.

- For segments with a rotation other than zero, there will be a hotspot at the origin of the segment (typically the segment's upper-left corner, not its center). This should allow the segment to be selected in the graphical display regardless of rotation whereas before for many rotation values it would not be able to be selected at all.

- Manual edits made to VIPP resources may be lost if the F5 ("temp save and refresh") key is not applied to the changed resource before performing other operations.  This is because execution is applied to the last known good state of all resources, which in part relies on the user indicating that good state by pressing the F5 key after edits. For this reason, use of the F5 key for manually changed resources is required before performing manual edits in other resources or operations in the right side graphical display. Note: this is not a change in behavior, just a clarification of the current behavior.


****************************************************
**** FreeFlow(R) VI Compose 15.0b Release Notes ****
****************************************************

The full README file for VI Compose can be found in 'X:\vide\xgf\' folder,
where 'X:' is the disk drive where FreeFlow VI Design Pro was installed.


***********************************************
**** Contents of Previous Release Versions ****
***********************************************
-------------------------------------
** Contents of 14.0 Service Pack 2 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 14.0e release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fixed AR #65195: When using Large Fonts, GEP Key box becomes 
  truncated in SHROW and BEGINTABLE SmartEditor GUI dialogs.

- Fixed AR #65328: VIPP source code is not updated after selecting
  an image file for the CellImage option in SHROW command dialog.
  
- Fixed AR #65352: Empty /CellImage parameter is added to the
  VIPP code when user simply checks /CellImage and clicks OK.

- Fixed AR #65354: Validation message is not shown as expected
  for "[/CLEAR Pattern]" or "[(CLEAR) Pattern]" syntax for the
  /CellFill parameter value.

- Fixed AR #65359: VDP crashes when editing DDG Chart Parameters
  using the SmartEditor GUI dialog.

- Fixed AR #65360: /Rotate parameter value is always reset to 0
  even when the VIPP code has value of 90, 180 or 270.

- Enhancement in VSI module so that SHOWFONT command can display
  on multiple pages all characters available, especially when the
  font character set includes thousands of glyphs.

It also includes all the previous 14.0 SP1 release contents.


-------------------------------------
** Contents of 14.0 Service Pack 1 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 14.0b release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fixed AR #65119: VDP crashes when a VPC file is loaded
  while SmartEditor is opened and then later is closed.

- Fixed AR #65261: Added /IAlign parameter for support of
  /CellImage in SHROW command dialog panel.

It also includes all the previous 14.0 release contents.


--------------------------------------
** Contents of 14.0 Release Version **
--------------------------------------

New Features
==================
- Upgrade to FreeFlow VI Compose (VIC) 14.0 release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Upgrade to Adobe PDFL SDK 11.0.3 version.

- Upgrade to Adobe APC SDK 1.1 version.

Enhancements
==================
- SmartEditor includes support for SHROW, BEGINTABLE, PDFBOUND
  and REPEAT commands.

- Modified SmartEditor for SETLAYOUT and DEFINELAYOUT /Rotate
  parameter which can also be defined as an array.

- Added highlighting of sections of code when enclosed in braces
  or curly brackets, IF/ENDIF and CASE/ENDCASE command keywords.
  For instance, clicking on a line containing an opening brace
  "{" or IF keyword will automatically highlight the section of
  code encapsulated with the corresponding closing brace "}" or
  ENDIF keyword. 

- Added keyboard shortcut Ctrl+Alt+C for "Mark text as comments"
  menu action.

- Added keyboard shortcut Ctrl+Alt+U for "Unmark commented text"
  menu action.

Bug Fixes
==================
- None.


-------------------------------------
** Contents of 12.1 Service Pack 1 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 12.1b release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fixed AR #64938: Race condition occurred when embedding EPS 
  into PDF with multiple/complex pages. This in turn resulted
  in displaying the default "text pattern" proxy in the PDF 
  rather than the intended EPS image.

  It also includes all the previous 12.1 release contents.

--------------------------------------
** Contents of 12.1 Release Version **
--------------------------------------

New Features
------------
- Upgrade to FreeFlow VI Compose (VIC) 12.1 release.
  See 'X:\vide\xgf\README.txt' file for release details.

Bug Fixes
------------
- None.

-------------------------------------
** Contents of 12.0 Service Pack 1 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 12.0a release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Installation support on Windows 8.1 and Windows Server 2012 R2 versions.

- Fixed SPAR #CQGbl00483929: An error was raised when attempting to
  display job resources located on a remote server referenced thru
  UNC paths in the xgfdos.run file.

- Fixed AR #64840: When using the Japanese language version, the 30-day
  "License Expiration" warning message also showed up in English when 
  installing or updating the software license.

- Fixed AR #64860: UNC paths in xgfdos.run did not work when exporting
  a job to PDF so an error was generated.

- Fixed AR #64891: In the Proof Print dialog, the OK button was enabled
  even when no PS printers are found to populate the top menu box.

  It also includes all the previous 12.0 release contents.

--------------------------------------
** Contents of 12.0 Release Version **
--------------------------------------

New Features
------------
- Upgrade to FreeFlow VI Compose (VIC) 12.0 release.
  See 'X:\vide\xgf\README.txt' file for release details.

Enhancements
------------
- Increased height of the Operand Stack & Dictionary Stack 
  windows that are shown when a PS or VIPP error occurs.
  This allows more trace/debug data to be readily visible
  for analyzis to try to figure out the cause of the error.

- Generic ZSORT options are available for non-database mode
  VIPP jobs; however, when using Generic ZSORT, the hotspot
  frames for objects in the right-hand side graphical window
  are no longer available. If you want to have hotspots to 
  move/edit objects on the page you can temporarily comment
  out the ZSORT command, and after making changes you can put
  the ZSORT command back.

Bug fixes
---------
- AR #64715: VDP misses embedded PDF resources while canceling 
  to open a job.

- AR #64716: Crash recovery did not work with PDF resources.

-------------------------------------
** Contents of 11.0 Service Pack 3 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 11.0e release.
  See 'X:\vide\xgf\README.txt' file for release details.

  It also includes all the previous 11.0 SP2 release contents..

-------------------------------------
** Contents of 11.0 Service Pack 2 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 11.0d release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fix for SPAR #736181436: Application freezes and stops responding
  when trying to do a "Copy&Paste" operation in certain conditions.

- When "Export Job as PDF" is selected, the GUI panel has a new
  "Include media and finishing options" checkbox which allows users
  to optionally include in the PDF all media and finishing options
  that have been defined and enabled for the job. Keep in mind that
  this option is intended to work only with FreeFlow Print Server
  devices using Adobe PDF Print Engine (APPE). On any other printers
  the new option is ignored and will have no effect. Also, note that
  selecting this option will increase the size of the resulting PDF,
  sometimes significantly depending on number of media changes and
  finishing options defined for the job and on the total number of
  pages.

- Fix for AR#64533: PS error displayed when SmarEditing GETPKeys for 
  /CellStroke parameter in a table definition.

- Fix for AR #64755: The tooltip message for "Create VI Project from
  currently loaded application" sometimes appeared with garbled text.

  This SP2 also includes all the previous 11.0 SP1 release contents.

-------------------------------------
** Contents of 11.0 Service Pack 1 **
-------------------------------------

- Upgrade to FreeFlow VI Compose (VIC) 11.0b release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Updated with VPSDK 2.1.3 version.

- Installation support on Windows 8 and Windows Server 2012.
  
- Fix for AR #64682: In some situations when using a system
  with two screen monitors, the "Export Job as PDF" options
  menu height was not displayed correctly.

  It also includes all the previous 11.0 release contents.

--------------------------------------
** Contents of 11.0 Release Version **
--------------------------------------

New Features
------------
- Upgrade to FreeFlow VI Compose (VIC) 11.0a release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Support for Hebrew and Arabic in resource editor and Smart Editor.
   1. Selecting a single word in the resource editor may cause the contents
      to appear to be modified. The workaround is to select text using the
      Smart Editor.
   2. After modifying the resource, the refresh command may not work. The 
      workaround is to close the document and reopen it.

Enhancements
------------
- Menu option to Export Job as PDF.

Bug fixes
---------
- AR #64658: In certain situations when loading a very large
  data submission file (over 4 MBytes), the program would crash.

- AR #64660: Sometimes when the PDF conversion to EPS failed,
  there was a "completed successfully" message still shown.

- AR #64672: When running in Demo mode, the license type showed
  as "Invalid License" in the License Dialog. This was incorrect.

-----------------------------------
** Contents of 10.1 Service Pack **
-----------------------------------

- Upgrade to FreeFlow VI Compose 10.0d release.
  See 'X:\vide\xgf\README.txt' file for release details.

- PDF files can be used as resources called directly from a VIPP job.

- Menu option to convert PDF to EPS files.

- Fix for SPAR #327104637: License Key activation failed when a
  product Serial Number is required. Now a "Serial Number" can
  be entered in the license GUI panel if required.

- The License Activation dialog panel includes a menu option to
  select one of the available MAC addresses for license HostID.
  The default menu selection is usually recommended.

- The PageUp, PageDown, Home and End buttons have been enabled for
  page navigation.

- Fix for AR #64608: Adding an XJT resource to a VI Project gets 
  incorrect type (it was set to "oth" but it should be "jdt").

- Fix for ARs #64644 & #64645: VPF XML entry for RESOURCE is malformed
  when filename contains an apostrophe (e.g. "CEO's Signature.eps").
  This would cause an error when creating or loading the VPC file.

-------------------------------------
** Contents of 10.0 Service Pack 2 **
-------------------------------------

- Upgrade to FreeFlow VI Compose 10.0c release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fine-tuned character encoding detection code that was giving 
  false positives when loading some job resource files.

- Updated DBM Wizard module to increase maximum number of data
  records to 500 and maximum number of data fields to 1200 when
  generating a sample data file.

  It also includes all the previous 10.0 SP1 release contents.

-------------------------------------
** Contents of 10.0 Service Pack 1 **
-------------------------------------

- Upgrade to FreeFlow VI Compose 10.0a release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fix for AR #64510: Licensing issue when installing on Windows OS
  version localized to Japanese, Chinese or Korean.

- Fix for AR #64514: Japanese char data using UTF8 encoding gets
  corrupted after any command editing with SmartEditor. 

- Fix for AR #64520: Unable to open SmartEditor on certain jobs
  containing Japanese char data using UTF8 encoding.

- Fix for AR #64521: Some new VIPP commands (BBOX, EXPAND, EXTVAR,
  SETOTL) are not highlighted in the left-hand-size source window.

- Fix for AR #64544: SmartEditor dialog for STOREVAR contains a
  reference to "VIPO" rather than the new "VIeC" acronym.

  It also includes all the previous 10.0 release contents.

-------------------------------------
** Contents of 9.0 Service Pack 3 **
-------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 9.0g release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fixed license HostID issue due to not detecting the MAC address of
  new NIC devices found on latest Windows Vista and Windows 7 PCs.

  Fixed the following ARs:

- AR #63688: Hot spot size issues for rotated objects in right-hand
  side graphical window.

- AR #64269: Duplicate resources displayed in ProofPrint dialog.

- AR #64467: SmartEditor produced error when editing PDF417 barcode.

- AR #64477: Application crashed when pressing Delete key within an
  empty source window, or when the cursor is at the very end of the
  source file.

  It also includes all the previous 9.0 SP2 release contents.

------------------------------------
** Contents of 9.0 Service Pack 2 **
------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 9.0d release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fix for SPAR #734544850: Slight shift when rendering a data/form 
  combination layout on A4 media size.

- Fix for AR #64449: String parameter inserted by PDF417 or QRCODE
  SmartEditor dialog was not enclosed in parenthesis and an error
  was generated.

  It also includes all the previous 9.0 SP1b release contents.

-------------------------------------
** Contents of 9.0 Service Pack 1b **
-------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 9.0b release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Fix for SPAR #945948969: Strings like "(...) ~RT SH" were displayed as
  part of the data for some jobs loaded and rendered in VI Designer.

- Fix for AR #64430: The second toolbar icon images shown for "Underline"
  and "Font and Effects Preview" features were incorrect so the expected
  functionality when clicking on the icons appears to be wrong.

- Fix for AR #64442: Append buttons in SmartEditor dialogs for PDF417 and
  QRCODE commands do not work when creating and inserting a new barcode.

- Fix for AR #64446: "Font Foreground Color" icon on the second toolbar
  was no longer updated with the correct color after selecting one of 
  the UV_2L_* ColorKeys.

  It also includes all the previous 9.0 SP1 release contents.

------------------------------------
** Contents of 9.0 Service Pack 1 **
------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 9.0a release.
  See 'X:\vide\xgf\README.txt' file for release details.

- Removed PS printers using the "Print to File" port from the list
  of printers in Proof Print dialog when running on Windows Vista, 
  Windows 7 and Windows 2008 Server.

- Fix for AR #64414: SmartEditor was deleting or incorrectly changing
  the VIPP source code for QRCODE and PDF417 barcodes.

- Fix for AR #64424: SmartEditor dialog for SETFINISHING command was
  displaying wrong menu selections.

- Fix for SPAR #049297555 & AR #64427: SmartEditor generates an error
  message when trying to edit INDEXFONT command that has GLT or MPR
  built-in variable used for Specialty Imaging fonts.

- Fix for AR #64428: VID crashed when opening a VPC containing files
  with a comma in the filenames.

  It also includes all the previous 9.0 release contents.

------------------------------------
** Contents of 8.0 Service Pack 3 **
------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 8.0d release.
  (See 'X:\vide\xgf\README.txt' file for release details).

- Fix for AR #64344: Could not select and move object on form resource
  from right-hand-side Graphical Window.

  It also includes all the previous 8.0 SP2 release contents.

------------------------------------
** Contents of 8.0 Service Pack 2 **
------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 8.0b release.
  (See 'X:\vide\xgf\README.txt' file for release details).

- Fix for AR #64308: Japanese message not displayed correctly in GUI.

- Fix for AR #64310: Japanese message not displayed correctly in GUI.

  It also includes all the previous 8.0 SP1a release contents.

-------------------------------------
** Contents of 8.0 Service Pack 1a **
-------------------------------------

- AR #64294: Licensing software was ignoring MAC addresses that begin
  with a non-zero value.

- AR #64307: Using the keyboard shortcut "Alt-v" in the s/w installer
  resulted in going to the next page of the installation wizard.

  It also includes all the previous 8.0 SP1 release contents.

------------------------------------
** Contents of 8.0 Service Pack 1 **
------------------------------------

- Upgrade to FreeFlow VI Interpreter (VII) 8.0a release.
  (See 'X:\vide\xgf\README.txt' file for release details).

- Installation support on Windows 7 OS.

- Installation support on 64-bit Windows platforms.
  
- Installation of Specialty Imaging (SI) screen fonts intended for
  viewing purposes ONLY. This allows you to load and view on screen
  VIPP jobs that use SI fonts without having to install SI printer 
  fonts in the VI Designer application. However, note that you do 
  still need to have the SI printer fonts installed on the printer
  device to be able to print the VIPP job and get the desired SI 
  font effects.

- Fix for AR #64269: Missing fonts when using ProofPrint feature.

----------------------------------------------------------------------
