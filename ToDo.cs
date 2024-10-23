namespace ToDoApi
{
    public class ToDoItem
    {
        // Eindeutige Identifikationsnummer für das ToDo-Element.
        public int Id { get; set; }
        // Die Aufgabe oder Beschreibung des ToDo-Elements.
        public string Task { get; set; }
        // Gibt an, ob das ToDo-Element abgeschlossen ist oder nicht.
        public bool IsCompleted { get; set; }
    }
}
