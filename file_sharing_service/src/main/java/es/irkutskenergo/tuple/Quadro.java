/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.tuple;

/**
 * Класс для создания кортежа
 *
 * @author Глеб
 */
public class Quadro<Param1, Param2, Param3, Param4> {

    public final Param1 param1;
    public final Param2 param2;
    public final Param3 param3;
    public final Param4 param4;

    public Quadro(Param1 param1, Param2 param2, Param3 param3, Param4 param4)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
    }

    @Override
    public String toString()
    {
        return "(" + param1 + ", " + param2 + ", " + param3 + ", " + param4 + ")";
    }

    @Override
    public boolean equals(Object other)
    {
        if (other == this)
        {
            return true;
        }

        if (!(other instanceof Quadro))
        {
            return false;
        }

        Quadro<Param1, Param2, Param3, Param4> other_ = (Quadro<Param1, Param2, Param3, Param4>) other;

        return other_.param1.equals(this.param1)
                && other_.param2.equals(this.param2)
                && other_.param3.equals(this.param3)
                && other_.param4.equals(this.param4);
    }

    @Override
    public int hashCode()
    {
        final int prime = 31;
        int result = 1;
        result = prime * result + ((param1 == null) ? 0 : param1.hashCode());
        result = prime * result + ((param2 == null) ? 0 : param2.hashCode());
        result = prime * result + ((param3 == null) ? 0 : param3.hashCode());
        result = prime * result + ((param4 == null) ? 0 : param4.hashCode());
        return result;
    }
}
