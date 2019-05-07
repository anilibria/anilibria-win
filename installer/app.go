package main

import (
	"os/exec"
)

func main() {
	cmd := exec.Command("runinstaller.bat")
	cmd.Start()
	cmd.Wait()
}

