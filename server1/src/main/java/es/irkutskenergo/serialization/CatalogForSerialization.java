/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
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
