'   =========================================================
'   RGB LED Sequencer (RgbLedS)
'
'   Copyright (c) Adrian John Dunstan. All rights reserved.
'
'   Licensed under the Apache License, Version 2.0 (the "License")'
'   you may not use this file except in compliance with the License.
'   You may obtain a copy of the License at
'
'       http://www.apache.org/licenses/LICENSE-2.0
'
'   Unless required by applicable law or agreed to in writing, software
'   distributed under the License is distributed on an "AS IS" BASIS,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'   See the License for the specific language governing permissions and
'   limitations under the License.
'   =========================================================

'   ---------------------------------------------------------
'   RgbLedS v1.1.0 - 2017-03-08
'   ---------------------------------------------------------

'   The RGB LED Sequencer is a 5 RGB LED device which allows up to 10 user
'   programmed light sequences to be stored in EEPROM. Sequences are sent to
'   the device via serial communication and each sequence may have up to
'   770 steps each with its' own specified delay time before the next step.
'   A user may press a button on the device which will cycle between the 10
'   currently programmed sequences; holding this button down for more than ~2.5
'   seconds will send the device into sleep (low power) mode - pressing the button
'   again or receiving a command signal via serial will wake the device again.

'Symbol names
'   LC = 25LC1024 1MBit EEPROM
'   SEGDRIVE = CD4543B BCD to 7 Segment Display Driver
'   TLC = TLC5940 16 Channel LED Driver Grayscale PWM Control

#picaxe 18m2
#no_end
setfreq m32

'The default TLC5940 grayscale pwm (when the selected sequence has 0 steps). Each
'channel stored as 2 bytes in little endian (last 12 bits of word used per channel).
'Valid grayscale pwm values are 0-4095.
eeprom (%11111111, %1111) 'B5
eeprom (%00000000, %0000) 'G5
eeprom (%11111111, %1111) 'R5
eeprom (%00000000, %0000) 'B4
eeprom (%11111111, %1111) 'G4
eeprom (%11111111, %1111) 'R4
eeprom (%11111111, %1111) 'B3
eeprom (%00000000, %0000) 'G3
eeprom (%00000000, %0000) 'R3
eeprom (%00000000, %0000) 'B2
eeprom (%11111111, %1111) 'G2
eeprom (%00000000, %0000) 'R2
eeprom (%00000000, %0000) 'B1
eeprom (%00000000, %0000) 'G1
eeprom (%11111111, %1111) 'R1
'The default TLC5940 dot correction (on program download). This can be changed with a
'CMDI_SETDOTCORRECTION command. Valid dot correction values are 0-63. Dot correction
'allows you to compensate for brightness differences in LEDs.
eeprom (63) 'B5
eeprom (63) 'G5
eeprom (63) 'R5
eeprom (63) 'B4
eeprom (63) 'G4
eeprom (63) 'R4
eeprom (63) 'B3
eeprom (63) 'G3
eeprom (45) 'R3
eeprom (63) 'B2
eeprom (63) 'G2
eeprom (43) 'R2
eeprom (63) 'B1
eeprom (63) 'G1
eeprom (63) 'R1

'Pin names
symbol LC_NOT_CS_PIN = B.5
symbol LC_SO_PINVALUE = pinB.4
symbol LC_SI_PIN = C.6 'Shared bus
symbol LC_SI_PINVALUE = outpinC.6
symbol LC_SCK_PIN = B.7
symbol TLC_SIN_PIN = C.6 'Shared bus
symbol TLC_SIN_PINVALUE = outpinC.6
symbol TLC_BLANK_PIN = B.3
symbol TLC_GSCLK_PIN = B.6
symbol TLC_VPRG_PIN = C.1
symbol TLC_SCLK_PIN = C.0
symbol TLC_XLAT_PIN = C.7
symbol SEGDRIVE_A_PIN = C.2
symbol SEGDRIVE_A_PINVALUE = outpinC.2
symbol SEGDRIVE_B_PIN = B.0
symbol SEGDRIVE_B_PINVALUE = outpinB.0
symbol SEGDRIVE_C_PIN = B.2
symbol SEGDRIVE_C_PINVALUE = outpinB.2
symbol SEGDRIVE_D_PIN = B.1
symbol SEGDRIVE_D_PINVALUE = outpinB.1
symbol BREAKINTERRUPT_PINVALUE = pinC.4
symbol BUTTONINTERRUPT_PINVALUE = pinC.5

'25LC1024 1Mbit EEPROM instruction set (sent over SPI Bus)
symbol LC_READ = %00000011 'Begin read, starting at selected address
symbol LC_WRITE = %00000010 'Begin write, starting at selected address
symbol LC_WREN = %00000110 'Set write enable latch (enable write)
symbol LC_WRDI = %00000100 'Reset write enable latch (disable write)
symbol LC_RDSR = %00000101 'Read STATUS register
symbol LC_WRSR = %00000001 'Write STATUS register
symbol LC_PE = %01000010 'Page erase
symbol LC_SE = %11011000 'Sector erase
symbol LC_CE = %11000111 'Chip erase
symbol LC_RDID = %10101011 'Release from deep power-down and read electronic signature
symbol LC_DPD = %10111001 'Deep power-down

'Serial In instruction set (instructions that can be received by this)
symbol CMDI_CONTINUE = %00000000 'Continues from the break state (used to wake the device without performing a command)
symbol CMDI_SETDOTCORRECTION = %00000001 'Save TLC5940 dot correction to the PICAXE EEPROM
symbol CMDI_PLAYSEQUENCE = %00000010 'Plays the sequence at a specified sequence number (0-9)
symbol CMDI_SAVESEQUENCE = %00000011 'Save a specified sequence to the 25LC1024 EEPROM
symbol CMDI_SLEEP = %00000100 'Sets the device to sleep mode (TLC5940 BLANK, 7 segment invalid to blank, 25LC1024 power down)
symbol CMDI_HANDSHAKE = %0000101 'Finish handshaking and receive a command
symbol CMDI_LCCLEAR = %00000110 'Clear the 25LC1024 EEPROM
symbol CMDI_READSEQUENCE = %00000111 'Read the sequence at a specified sequence number (0-9) from the device
symbol CMDI_READDOTCORRECTION = %00001000 'Read the dot correction data from the device

'Serial Out instruction set (instructions that can be sent by this)
symbol CMDO_READY = %00010000 'Signals that the device is ready for more data
symbol CMDO_HANDSHAKE = %00010001 'Signals that the device has responded to a break and is waiting for a handshake

'Interrupt conditions
symbol INT_BREAK = %00100000 'Breakstate
symbol INT_NEXTSEQUENCE = %00100001 'Change to next sequence
symbol INT_TOGGLESLEEP = %00100010 'Toggle sleep state

'Constants
symbol FrequencyMultiplier = 8 'The multiplier of the PICAXE frequency overclock (4MHz -> 32MHz)
symbol ChannelDataStart = 0 'The PICAXE EEPROM address that TLC5940 default channel data starts
symbol ChannelDataEnd = ChannelDataStart + 29 'The PICAXE EEPROM address that TLC5940 default channel data ends
symbol DotCorrectionStart = ChannelDataEnd + 1 'The PICAXE EEPROM address that TLC5940 dot correction data starts
symbol DotCorrectionEnd = DotCorrectionStart + 14 'The PICAXE EEPROM address that TLC5940 dot correction data ends
symbol PreviousSequenceAddress = DotCorrectionEnd + 1 'The PICAXE EEPROM address the previous sequence to be played is stored
symbol LcPageSize = 256 'The size of a page in the 25LC1024 EEPROM
symbol TlcSettleTime = 250 * FrequencyMultiplier 'The time given for the TLC5940 to settle
symbol ReadTimeoutTime = 3000 * FrequencyMultiplier 'The default timeout for serrxd commands
symbol NewSequenceWaitTime = 300 * FrequencyMultiplier 'The time to wait before a new selected sequence starts
symbol LongButtonTime = 5 '(32MHz = time increments every 0.5s) The time that a short button press becomes a long button press
symbol SequenceCount = 10 'The number of sequences that can be stored
symbol ChannelCount = 15 'The number of channels used (5 LEDs * 3)
symbol StepSize = ChannelCount + 2 'The size of a step (in bytes) - (1 byte per channel) + 2 bytes for step delay
symbol SequenceStepCount = 770 'The max number of steps a sequence can have
symbol SequenceSize = SequenceStepCount * StepSize'The size of a sequence (in bytes)
symbol SequenceAndStepCountSize = SequenceSize + 2 'The size of a sequence + 2 bytes for step count

'Variables
symbol lcAddress0 = b25 'The current 24 bit address for the 25LC1024 first 8 bits
symbol lcAddress1 = w13 'The current 24 bit address for the 25LC1024 last 16 bits
symbol lcPageBoundaryCrossed = b24 'Boolean (non-zero = true) indicating a page boundary has been crossed in the 25LC1024
symbol clockedByte = b23 'A general purpose byte to be clocked in and out of connected devices
symbol isAsleep = b21 'Boolean (non-zero = true) indicting if the device is asleep
symbol interruptType = b20 'Which interrupt type has most recently occurred
symbol numberOfSteps = w8 'The number of steps in the current sequence
symbol currentStep = w7 'The current step number in the current sequence
symbol dotCorrectionClockedChannel = b13 'The current TLC5940 dot correction channel being clocked through
symbol grayscaleClockedChannel = w9 'The current TLC5940 grayscale pwm channel being clocked through
symbol sevenSegmentValue = b22 'BCD (lower 4 bits) value shown on the 7 segment display (0-9)
symbol isNewDotCorrection = b12 'Boolean (non-zero = true) indicating if new dot correction data has just been clocked
symbol currentChannelClocking = b11 'The current grayscale channel being clocked to the TLC5940
symbol isNewSequence = b10 'Boolean (non-zero = true) indicating if a new sequence was requested
'RAM variables (bxx overflow)
symbol timeoutOccurred = 28

'Initial device state setup
Setup:
    high LC_NOT_CS_PIN
    low LC_SI_PIN
    low LC_SCK_PIN
    low TLC_SIN_PIN
    high TLC_BLANK_PIN
    low TLC_GSCLK_PIN
    low TLC_VPRG_PIN
    low TLC_SCLK_PIN
    low TLC_XLAT_PIN
    low SEGDRIVE_A_PIN
    low SEGDRIVE_B_PIN
    low SEGDRIVE_C_PIN
    low SEGDRIVE_D_PIN
    'Give the TLC5940 time to settle
    pause TlcSettleTime
    read PreviousSequenceAddress, sevenSegmentValue
    if sevenSegmentValue >= SequenceCount then
        sevenSegmentValue = 0
    endif
    gosub LcStatusRegisterSetup
    gosub TlcDotCorrection
    gosub PrepareCurrentSequence
    if numberOfSteps = 0 then
        gosub TlcClockDefaultSequence
    endif
    gosub UpdateSegDisplay
    'We disconnect so that we can use the break signal to interrupt and notify an incoming command
    disconnect
    gosub TlcSetReferenceClock
    gosub SetInterrupt

'Infinitely looping main routine
Main:
    do
        gosub CheckInterrupts
        if isAsleep != 0 then
            nap 4
        else
            gosub AdvanceCurrentSequence
        endif
    loop

'Check for any interrupts
CheckInterrupts:
    do while interruptType != 0
        select case interruptType
            case INT_BREAK
                if isAsleep != 0 then
                    gosub WakeDevice
                endif
                gosub ReadCommand
            case INT_NEXTSEQUENCE
                if isAsleep = 0 then
                    gosub NextSequence
                endif
                gosub DebounceButton
            case INT_TOGGLESLEEP
                gosub ToggleSleep
                'Wait for button release
                do while BUTTONINTERRUPT_PINVALUE = 1 loop
                gosub DebounceButton
        endselect
        interruptType = 0
        'If a read timeout occurred try to clean up
        peek timeoutOccurred, b0
        if b0 != 0 then
            gosub AttemptCleanUp
            poke timeoutOccurred, 0
        endif
        'Restart the current sequence
        if isAsleep = 0 then
            gosub PrepareCurrentSequence
        endif
        gosub SetInterrupt
        'If the user is cycling through sequences, we pause for a short time before the sequence starts playing
        'so the LEDs don't flash erratically. The interrupt has been set again so the pause can be interrupted
        'to keep cycling through sequences.
        if isNewSequence != 0 then
            pause NewSequenceWaitTime
            isNewSequence = 0
        endif
        'If the current sequence is empty and we haven't had another interrupt, clock the default sequence through
        if isAsleep = 0 and numberOfSteps = 0 and interruptType = 0 then
            gosub TlcClockDefaultSequence
        endif
    loop
    return

'Advance the currently running sequence, if there is one
AdvanceCurrentSequence:
    if numberOfSteps > 0 then
        if currentStep > numberOfSteps then
            gosub PrepareCurrentSequence
        endif
        gosub TlcClockCurrentStep
    endif
    return

'Read a command from the serial port
ReadCommand:
    clockedByte = CMDI_CONTINUE
    'Send the handshake command
    sertxd(CMDO_HANDSHAKE)
    'Read the command that is being sent (after receiving handshake)
    serrxd[ReadTimeoutTime, ReadTimeout], (CMDI_HANDSHAKE), clockedByte
    select case clockedByte
        case CMDI_PLAYSEQUENCE
            gosub PlaySequence
        case CMDI_SAVESEQUENCE
            gosub SaveSequence
        case CMDI_SETDOTCORRECTION
            gosub SetDotCorrection
        case CMDI_READSEQUENCE
            gosub ReadSequence
        case CMDI_READDOTCORRECTION
            gosub ReadDotCorrection
        case CMDI_LCCLEAR
            gosub LcClear
        case CMDI_SLEEP
            gosub SleepDevice
    endselect
    return

'Change to the next sequence and update the 7 segment display (0-9 wrap)
NextSequence:
    inc sevenSegmentValue
    sevenSegmentValue = sevenSegmentValue % SequenceCount
    gosub UpdateSegDisplay
    isNewSequence = 1
    return

'Prepare the device for the current sequence
PrepareCurrentSequence:
    if sevenSegmentValue < SequenceCount then
        write PreviousSequenceAddress, sevenSegmentValue
        gosub LcBeginRead
        lcAddress0 = sevenSegmentValue ** SequenceAndStepCountSize
        lcAddress1 = sevenSegmentValue * SequenceAndStepCountSize
        gosub LcAddressClock
        'w2(b4+b5) = Read number of steps from 25LC1024 EEPROM (little endian order)
        gosub LcReadByte
        b4 = clockedByte
        gosub LcReadByte
        b5 = clockedByte
        numberOfSteps = w2
        if numberOfSteps > SequenceStepCount then
            numberOfSteps = 0
        endif
        currentStep = 1
    else
        'End the current read cycle
        gosub LcEndCommand
        numberOfSteps = 0
    endif
    gosub UpdateSegDisplay
    return

'Set the the current sequence to a specified sequence number
PlaySequence:
    sertxd(CMDO_READY)
    'The sequence number we are loading
    serrxd[ReadTimeoutTime, ReadTimeout], sevenSegmentValue
    if sevenSegmentValue >= SequenceCount then
        sevenSegmentValue = 0
    endif
    isNewSequence = 1
    return

'Save a sequence to the 25LC1024 at a specified sequence number
SaveSequence:
    gosub LcBeginWrite
    sertxd(CMDO_READY)
    'The sequence number we are saving to
    serrxd[ReadTimeoutTime, ReadTimeout], clockedByte
    lcAddress0 = clockedByte ** SequenceAndStepCountSize
    lcAddress1 = clockedByte * SequenceAndStepCountSize
    gosub LcAddressClock
    'w4(b8+b9) = The number of steps in the sequence (little endian order)
    sertxd(CMDO_READY)
    serrxd[ReadTimeoutTime, ReadTimeout], b8
    sertxd(CMDO_READY)
    serrxd[ReadTimeoutTime, ReadTimeout], b9
    clockedByte = b8
    gosub LcWriteByte
    clockedByte = b9
    gosub LcWriteByte
    for w3 = 1 to w4
        for b5 = 1 to StepSize
            sertxd(CMDO_READY)
            serrxd[ReadTimeoutTime, ReadTimeout], clockedByte
            gosub LcWriteByte
        next b5
    next w3
    gosub LcEndCommand
    return

'Sets the dot correction of the TLC5940
SetDotCorrection:
    for b0 = DotCorrectionStart to DotCorrectionEnd
        sertxd(CMDO_READY)
        serrxd[ReadTimeoutTime, ReadTimeout], clockedByte
        write b0, clockedByte
    next b0
    gosub TlcDotCorrection
    return
    
'Reads the current sequence at a specified sequence number
ReadSequence:
    gosub LcBeginRead
    sertxd(CMDO_READY)
    'The sequence number we are reading from
    serrxd[ReadTimeoutTime, ReadTimeout], clockedByte
    lcAddress0 = clockedByte ** SequenceAndStepCountSize
    lcAddress1 = clockedByte * SequenceAndStepCountSize
    gosub LcAddressClock
    'w2(b4+b5) = Read number of steps from 25LC1024 EEPROM (little endian order)
    gosub LcReadByte
    b4 = clockedByte
    gosub LcReadByte
    b5 = clockedByte
    if w2 > SequenceStepCount then
        w2 = 0
    endif
    'Transmit number of steps
    sertxd(b4)
    sertxd(b5)
    for w1 = 1 to w2
        for b1 = 1 to StepSize
            gosub LcReadByte
            sertxd(clockedByte)
        next b1
    next w1
    gosub LcEndCommand
    return
    
ReadDotCorrection:
    for b0 = DotCorrectionStart to DotCorrectionEnd
        read b0, clockedByte
        sertxd(clockedByte)
    next b0
    return

'Prepare the 25LC1024 for a read sequence
LcBeginRead:
    gosub LcWaitWriteInProgress
    gosub LcBeginCommand
    clockedByte = LC_READ
    gosub LcByteClockWrite
    return

'Read a byte from the 25LC1024 EEPROM
LcReadByte:
    gosub LcByteClockRead
    gosub LcIncrementAddress
    return

'Clock a byte from the 25LC1024 EEPROM - MSB first
LcByteClockRead:
    'Clock byte through to b0 (bit0 to bit7)
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    bit7 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit6 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit5 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit4 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit3 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit2 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit1 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    bit0 = LC_SO_PINVALUE
    pulsout LC_SCK_PIN, 1
    clockedByte = b0
    return

'Prepare the 25LC1024 for a write sequence
LcBeginWrite:
    gosub LcWaitWriteInProgress
    gosub LcBeginCommand
    clockedByte = LC_WREN
    gosub LcByteClockWrite
    pulsout LC_NOT_CS_PIN, 1
    clockedByte = LC_WRITE
    gosub LcByteClockWrite
    return

'Send a byte to be written to the 25LC1024 EEPROM, with page boundry checking
LcWriteByte:
    if lcPageBoundaryCrossed = 1 then
        b4 = clockedByte
        gosub LcPageBoundary
        clockedByte = b4
    endif
    gosub LcByteClockWrite
    gosub LcIncrementAddress
    return

'Clock a byte through to 25LC1024 EEPROM - MSB first
LcByteClockWrite:
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    LC_SI_PINVALUE = clockedByte & %10000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %01000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00100000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00010000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00001000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00000100 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00000010 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = clockedByte & %00000001 max 1
    pulsout LC_SCK_PIN, 1
    return

'Clock a 24 bit address through to 25LC1024 EEPROM - MSB first (first 7 MSBs are don't care bits)
LcAddressClock:
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    LC_SI_PINVALUE = lcAddress0 & %10000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %01000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00100000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00010000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00001000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00000100 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00000010 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress0 & %00000001 max 1
    pulsout LC_SCK_PIN, 1
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    LC_SI_PINVALUE = lcAddress1 & %1000000000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0100000000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0010000000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0001000000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000100000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000010000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000001000000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000100000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000010000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000001000000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000100000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000010000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000001000 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000000100 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000000010 max 1
    pulsout LC_SCK_PIN, 1
    LC_SI_PINVALUE = lcAddress1 & %0000000000000001 max 1
    pulsout LC_SCK_PIN, 1
    lcPageBoundaryCrossed = 0
    return

'Increment the current address that the 25LC1024 is on
LcIncrementAddress:
    inc lcAddress1
    'Check if page boundary crossed
    b0 = lcAddress1 % LcPageSize
    if b0 = 0 then
        lcPageBoundaryCrossed = 1
        'If lower 16 bits overflows then increment upper 8 bits
        if lcAddress1 = 0 then
            inc lcAddress0
            'If 17 bits overflow - loop back to start
            if lcAddress0 = %00000010 then
                lcAddress0 = 0
            endif
        endif
    endif
    return

'Begin new write cycle at new page
LcPageBoundary:
    gosub LcBeginWrite
    gosub LcAddressClock
    return

'Clear the 25LC1024 EEPROM
LcClear:
    gosub LcBeginCommand
    clockedByte = LC_WREN
    gosub LcByteClockWrite
    pulsout LC_NOT_CS_PIN, 1
    'Chip erase command
    clockedByte = LC_CE
    gosub LcByteClockWrite
    gosub LcEndCommand
    return

'Sets the 25LC1024 into deep power down mode
LcDeepPowerDown:
    gosub LcBeginCommand
    clockedByte = LC_DPD
    gosub LcByteClockWrite
    gosub LcEndCommand
    return

'Releases the 25LC1024 from deep power down mode
LcReleaseDeepPowerDown:
    gosub LcBeginCommand
    clockedByte = LC_RDID
    gosub LcByteClockWrite
    'RDID requires dummy address to be clocked and for the electronic signature to be clocked
    gosub LcAddressClock
    for b0 = 1 to 8
        pulsout LC_SCK_PIN, 1
    next b0
    gosub LcEndCommand
    return
    
'Waits for any writes in progress on the 25LC1024
LcWaitWriteInProgress:
    low LC_NOT_CS_PIN
    do
        pulsout LC_NOT_CS_PIN, 1
        clockedByte = LC_RDSR
        gosub LcByteClockWrite
        gosub LcByteClockRead
        'Write In Progress bit is bit 0
        clockedByte = clockedByte & %00000001
    loop while clockedByte = %00000001
    high LC_NOT_CS_PIN
    return
    
'Set up the 25LC1024 status register to set all addresses to non write protected
LcStatusRegisterSetup:
    gosub LcBeginCommand
    clockedByte = LC_WRSR
    gosub LcByteClockWrite
    clockedByte = %00000000
    gosub LcByteClockWrite
    gosub LcEndCommand
    return

'Begins a new instruction on the 25LC1024
LcBeginCommand:
    'Force any current instructions to end
    high LC_NOT_CS_PIN
    pause 1
    low LC_NOT_CS_PIN
    return

'Ends the currently running instruction on the 25LC1024 (!ChipSelect = high)
LcEndCommand:
    high LC_NOT_CS_PIN
    return

'Clock the current step through to the TLC5940
TlcClockCurrentStep:
    gosub TlcPrepareChannelClock
    'Clock channel 14-0
    for b1 = 1 to 15
        if interruptType != 0 then
            exit
        endif
        gosub LcReadByte
        'Scale from 0-255 to 0-4095
        grayscaleClockedChannel = clockedByte * 16
        gosub TlcClockChannel
        inc currentChannelClocking
    next b1
    if interruptType = 0 then
        gosub TlcLatchGrayscale
        'w1(b2+b3) = Step delay
        gosub LcReadByte
        b2 = clockedByte
        gosub LcReadByte
        b3 = clockedByte
        if w1 != 0 then
            'Overclocking the PICAXE affects pause time so we loop the pause the necessary number of times
            for b4 = 1 to FrequencyMultiplier
                pause w1
                if interruptType != 0 then exit
            next b4
        end if
        inc currentStep
    endif
    return

'Prepare for TLC5940 grayscale channels to be clocked through
TlcPrepareChannelClock:
    grayscaleClockedChannel = %000000000000
    'If the previous channel clock was interrupted, finish clocking data through (we don't care what data)
    'Picaxe for loops always run at least once so we need the if check
    if currentChannelClocking < 16 then
        for b0 = currentChannelClocking to 15
            gosub TlcClockChannel
        next b0
        gosub TlcLatchGrayscale
    end if
    'Clock empty channel 15
    gosub TlcClockChannel
    currentChannelClocking = 1
    return

'Clocks a 12 bit channel through to the TLC5940
TlcClockChannel:
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %100000000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %010000000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %001000000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000100000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000010000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000001000000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000100000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000010000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000001000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000000100 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000000010 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = grayscaleClockedChannel & %000000000001 max 1
    pulsout TLC_SCLK_PIN, 1
    return

'Clocks the dot correction through to the TLC5940
TlcDotCorrection:
    high TLC_VPRG_PIN
    'Clock empty channel 15
    dotCorrectionClockedChannel = %000000
    gosub TlcClockDotChannel
    for b0 = DotCorrectionStart to DotCorrectionEnd
        read b0, dotCorrectionClockedChannel
        gosub TlcClockDotChannel
    next b0
    low TLC_SIN_PIN
    pulsout TLC_XLAT_PIN, 1
    low TLC_VPRG_PIN
    isNewDotCorrection = 1
    return

'Clocks a 6 bit dot correction channel through to the TLC5940
TlcClockDotChannel:
    'We could use loops and bit shifting to use less code, but we need as much speed as possible
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %100000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %010000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %001000 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %000100 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %000010 max 1
    pulsout TLC_SCLK_PIN, 1
    TLC_SIN_PINVALUE = dotCorrectionClockedChannel & %000001 max 1
    pulsout TLC_SCLK_PIN, 1
    return

'Clock the default grayscale pwm brightness stored in the EEPROM to the TLC5940
TlcClockDefaultSequence:
    gosub TlcPrepareChannelClock
    'Clock channel 14-0
    for b0 = ChannelDataStart to ChannelDataEnd step 2
        if interruptType != 0 then
            exit
        endif
        read b0, word grayscaleClockedChannel
        gosub TlcClockChannel
        inc currentChannelClocking
    next b0
    if interruptType = 0 then
        gosub TlcLatchGrayscale
    endif
    return

'Latch grayscale data to the TLC5940
TlcLatchGrayscale:
    pwmout TLC_BLANK_PIN, off
    high TLC_BLANK_PIN
    pulsout TLC_XLAT_PIN, 1
    'The first grayscale data input cycle after dot correction requires an additional SCLK pulse after the
    'XLAT signal to complete the grayscale update cycle
    if isNewDotCorrection != 0 then
        pulsout TLC_SCLK_PIN, 1
        isNewDotCorrection = 0
    endif
    'If we are latching a full cycle and not finishing an interrupted one
    if currentChannelClocking = 16 then
        low TLC_BLANK_PIN
        gosub TlcSetReferenceClock
    endif
    return

'Setup the reference clock and blank clock for the TLC5940 - Approximate ratio of 4096 GSCLK to 1 BLANK
TlcSetReferenceClock:
    pwmout TLC_GSCLK_PIN, 1, 3 '4000000Hz at 50% @ 32MHz
    pwmout pwmdiv64, TLC_BLANK_PIN, 127, 5 '977Hz at 1% @ 32MHz
    return

'Toggle the device sleep status
ToggleSleep:
    if isAsleep != 0 then
        gosub WakeDevice
    else
        gosub SleepDevice
    endif
    return

'Sets the device to sleep mode
SleepDevice:
    pwmout TLC_GSCLK_PIN, off
    pwmout TLC_BLANK_PIN, off
    high TLC_BLANK_PIN
    gosub LcDeepPowerDown
    b0 = sevenSegmentValue
    'Set to an invalid value to blank the display
    sevenSegmentValue = %1111
    gosub UpdateSegDisplay
    sevenSegmentValue = b0
    disablebod
    isAsleep = 1
    return

'Wakes the device from sleep mode
WakeDevice:
    gosub LcReleaseDeepPowerDown
    gosub UpdateSegDisplay
    enablebod
    isAsleep = 0
    return

'Update the 7 segment display to the current sevenSegmentValue
UpdateSegDisplay:
    SEGDRIVE_A_PINVALUE = sevenSegmentValue & %0001 max 1
    SEGDRIVE_B_PINVALUE = sevenSegmentValue & %0010 max 1
    SEGDRIVE_C_PINVALUE = sevenSegmentValue & %0100 max 1
    SEGDRIVE_D_PINVALUE = sevenSegmentValue & %1000 max 1
    return

'Debounce a button press
DebounceButton:
    pause 80
    return

'Set the interrupt flags
SetInterrupt:
    'Interrupt on serial in high (should come from breakstate), OR button press
    setint OR %00110000, %00110000
    return
    
'A read timeout occurred, this should only be used as a timeout from another sub routine so it can
'pop the correct return point from the call stack
ReadTimeout:
    poke timeoutOccurred, 1
    return

'Try to clean up from a timeout
AttemptCleanUp:
    'End any instruction on the 25LC1024 EEPROM
    gosub LcEndCommand
    return

'Interrupt routine
Interrupt:
    pwmout TLC_BLANK_PIN, off
    high TLC_BLANK_PIN
    gosub DebounceButton
    if BREAKINTERRUPT_PINVALUE = 1 then
        'Wait for breakstate to clear
        do while BREAKINTERRUPT_PINVALUE = 1 loop
        interruptType = INT_BREAK
    else if BUTTONINTERRUPT_PINVALUE = 1 then
        'If asleep then just wake up on button press
        if isAsleep != 0 then
            interruptType = INT_TOGGLESLEEP
        else
            time = 0
            'Wait for button release or long button press
            do while BUTTONINTERRUPT_PINVALUE = 1 and time <= LongButtonTime loop
            if time > LongButtonTime then
                interruptType = INT_TOGGLESLEEP
            else
                interruptType = INT_NEXTSEQUENCE
            endif
        endif
    else
        'The break or the button press wasn't long enough to detect
        gosub SetInterrupt
        low TLC_BLANK_PIN
        gosub TlcSetReferenceClock
    endif
    return