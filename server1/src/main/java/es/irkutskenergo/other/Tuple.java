/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.other;

/**
 * Класс для создания двухместного кортежа
 *
 * @author Глеб
 */
public class Tuple<Param1, Param2> {

    public final Param1 param1;
    public final Param2 param2;

    public Tuple(Param1 x, Param2 y)
    {
        this.param1 = x;
        this.param2 = y;
    }

    @Override
    public String toString()
    {
        return "(" + param1 + "," + param2 + ")";
    }

    @Override
    public boolean equals(Object other)
    {
        if (other == this)
        {
            return true;
        }

        if (!(other instanceof Tuple))
        {
            return false;
        }

        Tuple<Param1, Param2> other_ = (Tuple<Param1, Param2>) other;
        return other_.param1.equals(this.param1) && other_.param2.equals(this.param2);
    }

    @Override
    public int hashCode()
    {
        final int prime = 31;
        int result = 1;
        result = prime * result + ((param1 == null) ? 0 : param1.hashCode());
        result = prime * result + ((param2 == null) ? 0 : param2.hashCode());
        return result;
    }
}