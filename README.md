# book-cover-designer
The purpose of this program was to create an example of simplified book cover designer to learn as much as possible about WinForms technology.

## Program description

#### Menu bar and new project
* On the top of the window there is a menu bar with 2 options: 'File' and 'Settings'; under 'File' there are 3 options, each accessible with a shortcut: 'New', 'Open' and 'Save'
* Under 'Settings' there is a single option 'Language' which can be extended and one of two languages (English and Russian) can be selected. After selecting an option the other one deselects.
* You can use the menu options by pressing the alt key and using letters corresponding to a given option
* Choosing the 'New' option displays a dialog box which allows to enter dimensions of a new cover:
  * The dialogbox can not be resized
  * Content of the dialogbox is divided into 2 columns and 4 rows
  * Pressing the cancel button or closing the dialog box does not create a new project
  * Pressing the OK button creates a new project with no title or author and all additional texts are removed as well as the colors are set to default
#### Displaying title and author
* Title and author's name from the text boxes are displayed on the front cover (on the right) and on the book's spine using 'Arial' font
* Front cover:
  * The default title's font size is 32pt
  * Title can take up to 1/3 of the cover's height and the whole width
  * Title's text is drawn with the left line alignement but the title should always be drawn on the center of the cover
  * Author's name is displayed below the title and can take up to a 1/6 of the cover's height
  * The default authors's font size is 24pt
  * Author's name is always centered
  * If the title or author's name would take up more space than available, it is drawn using the biggest possible size available
* Spine:
  * Title and author's name are also drawn on the spine, rotated 90 degrees
  * All sequences of whitespace characters from title are replaced with a single space
  * The available space on the spine is divided in half for the title and the author
  * Default font sizes are the same as for the book's cover and sizes adjust if not enough space is available
* Changing cover and text colors:
  * There is a new section on the right that allows to change the color of the cover and all texts
  * There are 2 labels and 2 buttons
  * The labels and buttons are organized in a 2 by 2 table with border
  * Labels' font size is 11pt
  * Clicking one of the two buttons opens a color picker dialog and allows to change the color of the cover's background and the color of the text
* Adding and editing texts:
  * Text box for adding text from lab part is removed
  * Pressing the 'Add text' button opens a new dialogbox for text editing
  * The dialogbox can not be resized
  * There is a numeric input field that allows to change text font size
  * There are 3 radiobuttons that control text alignement
  * The rest of the dialogbox is filled with a multline text box with a vertical scroll bar
  * Changing line alignement changes the alignement of the text in the text box
  * There are 2 buttons on the bottom of the dialog for cancelling and accepting text addition or modification
  * As before, after confirming the text addition, cursor changes to cross while over the picture box and clicking anywhere on the drawing area adds new text
  * Double clicking on any previously added text opens up the dialog box with all fields filled appropriately. Changing and accepting the changes with 'OK' button modifies the text
  * After text editing, the text should stay centered around the same point as before
* Selecting, moving and deleting additional texts:
  * Right clicking on any existing additional text selects it and a border is drawn around it
  * Only one text can be selected at a time
  * Color of the border is the inversed color of the cover's background color
  * Moving the mouse with the middle mouse button pressed moves selected text
  * Right clicking on any part of the drawing area not containing any text should deselect any already selected text
  * Pressing the delete key with a text selected removes that text
* Localization:
  * The application supports two languages (English and Russian).
  * The application's default language is English
  * Language can be changed using menu option (Settings->Language). Each text in the application should be changed to an equivalent in a different language.
  * After changing the localization all controls should be reloaded correctly
  * The main window should remain in the same position and be the same size
* Saving and loading the project:
  * The project can be saved and loaded by choosing the 'Save' and 'Open' options from the menu
  * The format of the file containing the saved project in this case is a simple .txt file
  * Every information is saved, including: title, author, additional texts (with font size and line alignement), background and text colors
  * To save or load a project an appropriate dialog box is opened
  * Save dialog should force default file extension (chosen appropriately)
  * Dialogbox should force user to load only files with the chosen extension
