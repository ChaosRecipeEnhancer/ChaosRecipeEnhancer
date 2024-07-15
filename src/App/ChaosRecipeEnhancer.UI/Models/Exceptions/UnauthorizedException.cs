using System;

namespace ChaosRecipeEnhancer.UI.Models.Exceptions;

public class UnauthorizedException(string message) : Exception(message) { }