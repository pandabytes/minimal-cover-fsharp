namespace MinimalCover.Domain

open System

module FunctionalDependency = 
  let private ValidateLeftAndRight (left: Set<string>) (right: Set<string>) = 
    if (left.Count > 0 && right.Count > 0) then
      let hasEmptyLeftAttrbs = Set.exists (String.IsNullOrWhiteSpace) left
      let hasEmptyRightAttrbs = Set.exists (String.IsNullOrWhiteSpace) right
      if (hasEmptyLeftAttrbs || hasEmptyRightAttrbs) then
        raise (ArgumentException "Both left and right must have non-null and non-empty attributes.")
    else
      raise (ArgumentException "Both left and right must have at least 1 attribute.")

  [<StructuredFormatDisplay("Left: {Left}; Right: {Right}")>]
  type T(left: Set<string>, right: Set<string>) = 
    do ValidateLeftAndRight left right
    member this.Left = left
    member this.Right = right

    interface IComparable with
      // < 1 -> before
      // = 0 -> same
      // > 1 -> after
      member this.CompareTo (obj: obj) =
        match obj with
          | :? T as otherFd ->
              compare (Set.union this.Left this.Right) (Set.union otherFd.Left otherFd.Right)
          | _ -> -1

    override this.Equals (obj: obj) =
      match obj with
        | :? T as otherFd ->
          (LanguagePrimitives.PhysicalEquality this otherFd) ||
          (this.Left.Equals(otherFd.Left) && this.Right.Equals(otherFd.Right))
        | _ -> false

    override this.GetHashCode() =
      let startingHashCode = 1430287
      let hashCode = Set.fold (fun prev current -> prev * current.GetHashCode()) startingHashCode this.Left
      (Set.fold (fun prev current -> prev * current.GetHashCode()) hashCode this.Right) * 17
