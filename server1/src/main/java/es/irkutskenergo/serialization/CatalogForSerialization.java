package es.irkutskenergo.serialization;

/**
 *
 * @author admin
 */
public class CatalogForSerialization {
    public String  name;
    public boolean file;
    public String  content;
    
    public CatalogForSerialization(String name, boolean file)
    {
        this.name = name;
        this.file = file;
        if(file) this.content = null;
    }
    
    public CatalogForSerialization(String name, boolean file, String content)
    {
            this.name = name;
            this.file = file;
            this.content = content;
    }
}
