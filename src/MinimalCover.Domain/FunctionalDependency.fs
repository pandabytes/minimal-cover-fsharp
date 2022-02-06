namespace MinimalCover.Domain

open System

module FunctionalDependency = 
  let private ValidateLeftAndRight (left: Set<string>) (right: Set<string>) = 
    if (left.Count > 0 && right.Count > 0) then
      let hasEmptyLeftAttrbs = Set.exists (fun a -> String.IsNullOrWhiteSpace a) left
      let hasEmptyRightAttrbs = Set.exists (fun a -> String.IsNullOrWhiteSpace a) right
      if (hasEmptyLeftAttrbs || hasEmptyRightAttrbs) then
        raise (ArgumentException "Both left and right must have non-null and non-empty attributes.")
    else
      raise (ArgumentException "Both left and right must have at least 1 attribute.")

  //type T = class
  //  val Left: Set<string>
  //  val Right: Set<string>

  //  new(left, right) = 
  //    ValidateLeftAndRight left right
  //    { Left = left; Right = right; }    
  //end

  type T(left: Set<string>, right: Set<string>) = 
    do ValidateLeftAndRight left right
    member this.Left = left
    member this.Right = right

  /// Check if 2 functional dependencies are equal.
  let AreEqual (fd1: T) (fd2: T) =
    if (LanguagePrimitives.PhysicalEquality fd1 fd2) then
      true
    else
      fd1.Left.Equals(fd2.Left) && fd1.Right.Equals(fd2.Right)

  